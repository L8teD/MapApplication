p_n = 99536;
p_d = 276;
dp_n = 0.0002 * p_n;
dp_d = 0.001 * p_d;
M = 0.063;

k = 1.4;


chisl = sqrt(2/k-1) * p_d / p_n
znamen = k * M*(p_d/p_n + 1)
mnoj = dp_d / p_d - dp_n/p_n

mnoj * chisl/znamen