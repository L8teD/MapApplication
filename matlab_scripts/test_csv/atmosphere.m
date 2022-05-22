clc;clear;close all;

dataTable = readtable('atmosphere.csv');
dataTable.Properties.VariableNames = {'h_geom', 'h_pot', 'h_pot2', 'T_kelvin', 'T_kelvin2', 'T_celc', 'p_Pa', 'p_Pa2', 'p_mm', 'ro', 'g', 'g2'};

h = GetDoubleFromCell(dataTable.h_geom(41:45));
p = str2double(dataTable.p_Pa(41:45));
t = dataTable.T_kelvin(41:45);
ro = str2double(dataTable.ro(41:45));
figure
plot(h,ro, 'b*');
grid on
function [double_array] = GetDoubleFromCell(cell)
    for i = 1:1:length(cell)
        y = strsplit(char(cell(i)));
        if (length(y) > 1)
            if (str2double(char(y(1))) > 0)
                double_array(i,1) = str2double(char(y(1))) * 1000 + str2double(char(y(2)));
            else
                double_array(i,1) = str2double(char(y(1))) * 1000 - str2double(char(y(2)));
            end
        else
            double_array(i,1) =  str2double(char(y(1)));
        end
    end
end