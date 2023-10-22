import balance_numpy
import sys

MODE_INTERACTIVE = True
MODE_INTERACTIVE = False

def process(s):
    print("OUTPUT:")
    try:
        eq = balance_numpy.Chemeq(s)
        print(eq.solve())
    except:
        print('ERROR: %s - %s' % (sys.exc_info()[0], sys.exc_info()[1]))
    print('-' * 80 + '\n')

if MODE_INTERACTIVE:  
    while (True):
        s = input("INPUT: ")
        if s == "q":
            break
        process(s)
else:
    with open("tests_manual.txt", "r") as f:
        eqs = []
        for line in f:
            if line[0] != ' ' and line[0] != '-':
                eqs.append(line)
    for s in eqs:
        print("INPUT: " + s)
        process(s)