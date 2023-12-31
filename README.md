# Chemical reaction equation balancing tool
This tool finds balancing coefficients for a chemical reaction equation. Relies on [Rationals](https://github.com/tompazourek/Rationals/tree/master)

## Syntax
... is very simple. Spaces are ignored. Letters, numbers, '(' and ')' are allowed. Decimal fractions like '1.8' are allowed for indices (commonly for berthollides).
There is no chemistry-specific knowledge in the code, so use any symbols you prefer. For example, 'Ln' for any lanthanoid, 'Ph' for phenol, etc.
<b>Note:</b> Qn and Qp are interpreted as ion charges. For instance, 'IQn3' is 'I{-3}' and 'HQp' is a proton.

### Samples
- N2O4 = NO2
- O2 = O3
- Ni(CO)4 = Ni + CO
- CaO + P2O5 = Ca3(PO4)2
- NH3 + H2SO4 = (NH4)2SO4
- Ca3(PO4)2 + H3PO4 = Ca(H2PO4)2
- NaHCO3 + Ca(H2PO4)2 = Na2HPO4 + CaHPO4 + CO2 + H2O
- (CH3)2N2H2 + N2H4 + N2O4 = CO2 + H2O + N2
- IO4Qn + IQn = IO3Qn + I3Qn + H2O + OHQn
- Fe2(SO4)3 + PrTlTe3 + H3PO4 = Fe0.996(H2PO4)2H2O + Tl1.987(SO3)3 + Pr1.998(SO4)3 + Te2O3 + P2O5 + H2S
- C2952H4664N812O832S8Fe4 + Na2C4H3O4SAu + Fe(SCN)2 + Fe(NH4)2(SO4)2(H2O)6 + C4H8Cl2S + C8H12MgN2O8 = C55H72MgN4 + Na3.99Fe(CN)6 + Au0.987SC6H11O5 + HClO4 + H2S
- CaBeAsSAtCsF13 + (Ru(C10H8N2)3)Cl2(H2O)6 + W2Cl8(NSeInCl3)2 + Ca(GaH2S4)2 + (NH4)2MoO4 + K4Fe(CN)6 + Na2Cr2O7 + MgS2O3 + LaTlS3 + Na3PO4 + x11 Ag2PbO2 + SnSO4 + HoHS4 + CeCl3 + ZrO2 + Cu2O + Al2O3 + Bi2O3 + SiO2 + Au2O + TeO3 + CdO + Hg2S =
(NH3)3((PO)4(MoO3)12) + LaHgTlZrS6 + In3CdCeCl12 + AgRuAuTe8 + C4H3AuNa2OS7 + KAu(CN)2 + MgFe2(SO4)4 + Sn3(AsO4)3BiAt3 + CuCsCl3 + GaHoH2S4 + N2SiSe6 + CaAl0.97F5 + PbCrO4 + H2CO3 + BeSiO3 + HClO + W2O