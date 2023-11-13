import sys, re
import numpy as np, sympy as sp
import functools, fractions

MODE_VECTORLABELS = False
MODE_GENERALFORM_ANY = False
MODE_SHOW_MATRICES = False

#MODE_VECTORLABELS = True
#MODE_GENERALFORM_ANY = True
MODE_SHOW_MATRICES = True

# MATH:
def lcm(a, b):
    return a * b // gcd(a, b)

def gcd(a, b, rtol = 0.001):
    t = min(abs(a), abs(b))
    while abs(b) > rtol * t:
        a, b = b, a % b
    return a

def scalevector(v):
    fs = [fractions.Fraction(n).limit_denominator() for n in v]
    multiple  = functools.reduce(lcm, [f.denominator for f in fs])
    ints      = [f * multiple for f in fs]
    divisor   = functools.reduce(gcd, ints)
    return [int(n / divisor) for n in ints]

def getlabels(x):
    if MODE_VECTORLABELS:
        return [f"x{i}" for i in range(1, x)]
    else:
        return [chr(i) for i in range(ord('a'), ord('a') + x)]

def reducematrix(m):
    result = np.array(sp.Matrix(m).rref()[0].tolist(), dtype=float)
    while np.count_nonzero(result[-1]) == 0:
            result = result[0:-1]
    return result

def getaugmentingrows(count, width):
    return np.hstack([np.zeros([count, width - count]), np.identity(count)])

def readable(eq):
    result = eq
    result = re.sub(r'\((\d+\w+)\)', r'\1', result)                 # избыточные скобки
    result = re.sub(r'(^| |\(|-)1(\D)', r'\1\2', result)            # единичные коэффициенты
    result = result.replace('+ -', '- ')
    return result

def getSLE(m_original):
    result = []

    m = reducematrix(m_original)
    labels = getlabels(m.shape[1])

    freevars = []
    i = 0
    for r in m.transpose():
        if np.count_nonzero(r) > 1:
            freevars.append(i)
        i = i + 1

    i = 0
    for r in m:
        j = i
        v = scalevector(r)
        while (v[j] == 0):
            j = j + 1
        if v[j] < 0:
            v = np.multiply(v, -1)
        parts = []
        for k in range(j + 1, len(v)):
            if v[k] != 0:
                parts.append(f"{-v[k]}{labels[k]}")
        scaled_form = ' + '.join(parts)
        if v[j] != 1:
            scaled_form = f"({scaled_form})/{v[j]}"
        result.append(readable(f"{labels[j]} = {scaled_form}"))
        i = i + 1
    result.append('for any rational ' + ', '.join(labels[x] for x in freevars))

    return '\n'.join(result)

# MAIN:
class Chemeq(object):
    cre = ''
    elements = None
    fragments = None
    m = None
    border = -1

    def m_height(self):
        return self.m.shape[0]

    def m_width(self):
        return self.m.shape[1]

    def m_rank(self):
        return np.linalg.matrix_rank(self.m)

    def m_nullity(self):
        return self.m_width() - self.m_rank()

    def plain(self, s):
        # раскрыть скобки без индекса
        regex = r'\(([^\)]*?)\)(\D|=|$)'
        while(True):
            o = re.search(regex, s)
            if o == None:
                break
            s = re.sub(regex, r'\1\2', s)

        # раскрыть вложенные скобки с индексом
        regex = r'(\(([^\)]*).*)\((.*?)\)(\d+)'
        while(True):
            o = re.search(regex, s)
            if o == None:
                break
            s = re.sub(regex, r'\1~', s, 1)
            s = s.replace('~', o.group(3) * int(o.group(4)))

        # раскрыть оставшиеся скобки (нулевого уровня) с индексом
        regex = r'\((.*?)\)(\d+)'
        while(True):
            o = re.search(regex, s)
            if o == None:
                break
            s = re.sub(regex, o.group(1) * int(o.group(2)), s, 1)

        # вставить индекс 1 там, где он нужен
        for element in self.elements:
            regex = r'(X)([A-Z]|\+|=|$)'.replace('X', element)
            while(True):
                o = re.search(regex, s)
                if o == None:
                    break
                s = re.sub(regex, r'\1~\2', s)
                s = s.replace('~', '1')
        return s

    def initcompositionmatrix(self):
        # построить матрицу для каждого элемент в каждом фрагменте
        matrix = np.empty([0, len(self.fragments)])
        for element in self.elements:
            regex = r'X(\d+(\.\d+)*)'.replace('X', element)
            row = []
            for fragment in self.fragments:
                x = 0.
                for y in re.findall(regex, self.plain(fragment)):
                    x = x + float(y[0])
                row = row + [x]
            matrix = np.vstack([matrix, row])
        self.m = matrix
        if MODE_SHOW_MATRICES:
            print(self.m)
            print(f"Matrix rank {self.m_rank()}, nullity {self.m_nullity()}")
        return

    def __init__(self, equation):
        self.cre = equation
        self.elements = list(set(re.findall(r'[A-Z][a-z]?', self.cre)))  
        self.fragments = [x.strip() for x in re.sub(r'\+|=', '|', self.cre).split('|')]
        self.border = len(re.sub(r'\+', '|', self.cre.split('=')[0]).split('|'))
        self.initcompositionmatrix()

    def getEqWithCoefficients(self, v):
        l = []
        r = []
        for i in range(0, len(v)):
            t = str(abs(v[i])) + self.fragments[i]
            if v[i] > 0:
                r.append(t)
            elif v[i] < 0:
                l.append(t)
            else:
                continue
        return readable(' + '.join(l) + ' = ' + ' + '.join(r))

    def getEqWithPlaceholders(self):
        v = getlabels(len(self.fragments))
        l = []
        for i in range(0, self.border):
            l.append(v[i] + self.fragments[i])
        r = []
        for i in range(self.border, len(v)):
            r.append(v[i] + self.fragments[i])
        return readable(' + '.join(l) + ' = ' + ' + '.join(r))

    def solve(self):
        result = []
        if self.m_nullity() == 0:
            raise Exception('This equation cannot be balanced')

        x = self.m_width() - self.m_height()
        if x <= 0:
            self.m = reducematrix(self.m)
            x = self.m_width() - self.m_height()

        try:
            IAM = np.linalg.inv(np.append(self.m, getaugmentingrows(x, self.m_width()), 0))
            if MODE_SHOW_MATRICES:
                print(IAM)
            singular_matrix = False
        except:
            IAM = None
            singular_matrix = True

        if MODE_GENERALFORM_ANY:
            singular_matrix = True

        if singular_matrix == False:
            if x > 1:
                result.append(f"{x} independent sets of balancing coefficients:")
            for i in range(0, x):
                tag = ''
                if x > 1:
                    tag = chr(ord('A') + i) + '): '
                NSV = IAM[:, -(i + 1)]
                v = scalevector(NSV.transpose())
                result.append(tag + self.getEqWithCoefficients(v))
        else:
            result.append(self.getEqWithPlaceholders())
            for r in self.m:
                for i in range(0, len(r)):
                    if i >= self.border and r[i] != 0:
                        r[i] = -1 * r[i]
            ZAM = np.append(self.m, np.zeros([self.m_height(), 1]) , 1)
            result.append(getSLE(ZAM))

        return '\n'.join(result)