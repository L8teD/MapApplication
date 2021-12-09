clc;clear;close all;

dataTable = readtable('covar.csv', 'Format', '%f%f%f%f%f%f');
dataTable.Properties.VariableNames = {'lon', 'lat', 'alt', 've', 'vn', 'vh'};

subplot(4,1,1)
plot(dataTable.lon)
grid on
title('lon')
subplot(4,1,2)
plot(dataTable.lat)
grid on
title('lat')


subplot(4,1,3)
plot(dataTable.ve)
grid on
title('ve')

subplot(4,1,4)
plot(dataTable.vn)
grid on
title('vn')
