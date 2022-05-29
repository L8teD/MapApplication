clc;clear;close all;

dataTable = readtable('velSVS.csv');
dataTable.Properties.VariableNames = {'E','N','H'};
E = dataTable.E;
x = 1:1:length(E);
x = x';
figure;
plot(x,E);
grid on;
hold on;
counter = 0;
N = 5;
coeff2 = polyfit(x, E, N);
output = 0;
for k=0:N
    d = x.^k;
    j = d * coeff2(N-k+1);
    output = output + j;
    counter = counter + 1;
end
counter
plot(x,output);
output(length(x), 1)