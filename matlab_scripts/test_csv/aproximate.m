clc; close all;

H = min(h):0.1:max(h);

T = AproximateAndPlot(h, t,'Temperature, [K]'); 
P = AproximateAndPlot(h, p,'Pressure, [Pa]'); 
RO = AproximateAndPlot(h, ro,'Density, [kg/m^3]'); 

function [output] = AproximateAndPlot(h, value, y_axis_name)
    figure
    grid on
    plot(h, value, 'or');
    Hi = min(h):0.1:max(h);
    output = Aproximate(h,Hi, value);
    hold on;
    plot(Hi, output, 'g');
    grid on
    ylabel(y_axis_name);
    xlabel("Altitude, [m]")
    legend('Input data','Aproximate data');
end
function [output] = Aproximate(H, Hi, value)
    N = 2;
    coeff2 = polyfit(H, value, N);
    output = 0;
    for k=0:N
        output = output + coeff2(N-k+1) * Hi.^k;
    end
end