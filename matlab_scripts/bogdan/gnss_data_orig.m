%формат альманаха:
%1 строка:
%1 столбец - число получения альманаха
%2 столбец - месяц получения альманаха
%3 столбец - год получения альманаха
%4 столбец - время получения альманаха от начала суток, с UTC
%5 столбец - неделя GPS (получения альманаха)
%2 строка%
%1 столбец - номер PRN
%2 столбец - обобщенный признак здоровья
%3 столбец - неделя GPS (альманаха)
%4 столбец - время недели GPS, с (альманаха)
%5 столбец - число
%6 столбец - месяц
%7 столбец - год
%8 столбец - время альманаха, с
%9 столбец - поправка времени КА GPS относительно системного времени, с
%10 столбец - скорость поправки времени КА GPS относительно системного времени, с/с
%11 столбец - Om0 - скорость долготы узла, полуциклы/c
%3 строка:
%1 столбец - Om0 - долгота узла, полуциклы
%2 столбец - I - наклонение, полуциклы
%3 столбец - w - аргумент перигея, полуциклы
%4 столбец - E - эксцентриситет
%5 столбец - SQRT(A) - корень из большой полуоси, м**0.5
%6 столбец - M0 - средняя аномалия, полуциклы
%перед запуском скрипта импортировать альманах через HOME/Import Data как матрицу,
%переименовать в "gnss_data", сохранить как "gnss_data.mat"
clc
clear
load('gnss_data.mat'); %имя файла
M = 0;
gnss_strings_1(1,1:length(gnss_data(1,:))) = 0;
gnss_strings_2(1,1:length(gnss_data(1,:))) = 0;
gnss_strings_3(1,1:length(gnss_data(1,:))) = 0;
for i = 1:length(gnss_data(:,1))
    if mod(i,3) == 1
        gnss_strings_1(i,:) = gnss_data(i,:);
    elseif mod(i,3) == 2
        gnss_strings_2(i,:) = gnss_data(i,:);
    else 
        gnss_strings_3(i,:) = gnss_data(i,:);
    end
end
k = 1;
for i = 1:length(gnss_strings_1(:,1))
    if sum(gnss_strings_1(i,:)) ~= 0
        gnss_strings_1_nonzero(k,:) = gnss_strings_1(i,:);
        k = k+1;
    end
end
k = 1;
for i = 1:length(gnss_strings_2(:,1))
    if sum(gnss_strings_2(i,:)) ~= 0
        gnss_strings_2_nonzero(k,:) = gnss_strings_2(i,:);
        k = k+1;
    end
end
k = 1;
for i = 1:length(gnss_strings_3(:,1))
    if sum(gnss_strings_3(i,:)) ~= 0
        gnss_strings_3_nonzero(k,:) = gnss_strings_3(i,:);
        k = k+1;
    end
end
gnss_strings_1 = gnss_strings_1_nonzero;
gnss_strings_2 = gnss_strings_2_nonzero;
gnss_strings_3 = gnss_strings_3_nonzero;

al_gp_t0 = gnss_strings_2(1,4);
al_gp_lan(length(gnss_strings_1(:,1)),1) = 0;
al_gp_i (length(gnss_strings_1(:,1)),1) = 0;
al_gp_w (length(gnss_strings_1(:,1)),1) = 0;
al_gp_e (length(gnss_strings_1(:,1)),1) = 0;
al_gp_sqrt_a (length(gnss_strings_1(:,1)),1) = 0;
al_gp_m0 (length(gnss_strings_1(:,1)),1) = 0;
al_gp_health (length(gnss_strings_1(:,1)),1) = int8(0);
for s = 1:length(gnss_strings_1(:,1))
    if gnss_strings_2(s,2) == 0
        al_gp_lan(s,1) = gnss_strings_3(s,1);
        al_gp_i(s,1) = gnss_strings_3(s,2);
        al_gp_w(s,1) = gnss_strings_3(s,3);
        al_gp_e(s,1) = gnss_strings_3(s,4);
        al_gp_sqrt_a(s,1) = gnss_strings_3(s,5);
        al_gp_m0(s,1) = gnss_strings_3(s,6);
        al_gp_lan(s,1) = al_gp_lan(s,1).*pi;
        al_gp_i(s,1) = al_gp_i(s,1).*pi;
        al_gp_w(s,1) = al_gp_w(s,1).*pi;
        al_gp_m0(s,1) = al_gp_m0(s,1).*pi;
        al_gp_health(s,1) = int8(0);
    else
        al_gp_health(s,1) = int8(1);
        M = M+1;
    end
end
al_gp_M = k;
al_gp_resolution = 1000;   %10000
al_gp_mu = 3.986004418E+14; %1.32712440041E+20;
%максимальное кол-во возможных видимых спутников

clearvars -EXCEPT al*