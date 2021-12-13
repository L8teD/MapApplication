clc;clear;close all;

dataTable = readtable('x_dot.csv', 'Format', '%f%f%f%f%f%f');
dataTable.Properties.VariableNames = {'lon', 'lat', 'alt', 've', 'vn', 'vh'};
figure
% subplot(4,1,1)
plot(dataTable.lon)
grid on
title('lon')
figure
% subplot(4,1,2)
plot(dataTable.lat)
grid on
title('lat')

figure
% subplot(4,1,3)
plot(dataTable.ve)
grid on
title('ve')

% subplot(4,1,4)
figure
plot(dataTable.vn)
grid on
title('vn')