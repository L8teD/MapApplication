% clear
% clc
tr_Vp = 100/3.6; %скорость полета [м/с]
roll_turn = 10*pi/180; %гамма координированная [рад] (крен)
d_roll = 0*pi/180;  %угловая скорость крена
teta_turn = 15*pi/180; %Тангаж
R_a = 6378136;    %большая полуось Земли ПЗ-90.11 [m]
R_b = 6356751.3618;   %малая полуось Земли ПЗ-90.11 [m]
R_ge = 9.78049; %на экваторе [m/c^2]
%%коэффициенты в формулах ускорения свободного падения
R_q = 0.00346775;
R_betta = 0.0053171;
R_betta1 = 71 * 10^-7;
R_turn_roll = (tr_Vp^2)/(R_ge*tan(roll_turn));     %радиус коорд. разворота в горизонтальной пл-ти
R_turn_pitch = tr_Vp^2/(R_ge*tan(roll_turn));     %радиус коорд. разворота в вертикальной пл-ти
R_e2 = 0; %0.0066943662;      %квадрат эксцентриситета ОЗЭ ПЗ-90.11
R_e12 = 0; %0.0067394828;      %квадрат второго эксцентриситета ОЗЭ ПЗ-90.11
R_alpha = 0; %1/298.25784;      %степень сжатия ОЗЭ ПЗ-90.11
R_u = 7.292115*10^(-5); %Скорость вращения земли вокруг своей оси ОЗЭ ПЗ-90.11 [рад/с]
R_rz = 6378163;
dT = 1;   %шаг расчета

% flight_mission = [30, 30.5, 30.5, 30, 30, 30.5, 30.5, 30, 30, 30.5, 30.5, 30, 30,;         %полетное задание [широты;   
%                  30, 30, 30.5, 30.5, 30, 30, 30.5, 30.5, 30, 30, 30.5, 30.5, 30;          %                  долготы;
%                  5000, 5000, 5000, 5000, 5000, 5000, 5000, 5000, 5000 5000, 5000, 5000, 5000];     %                  высоты]
% 
flight_mission = [53.47, 55, 56, 57, 53.47, 53.97, 53.97, 53.47, 53.47;         %полетное задание [широты;   
                 58.77, 58.77, 59.27, 59.27, 58.77, 58.77, 58.27, 58.27, 59.77;          %                  долготы;
                 5000, 5000, 5000, 5000, 5000, 5000, 5000, 5000, 5000];     %                  высоты]

%высота изменяется после поворота по курсу
N_point = size(flight_mission,2);   %число точек маршрута

for k = 1:N_point
    Fi(k) = flight_mission(1,k)*pi/180;
    Lam(k) = flight_mission(2,k)*pi/180;
    U(k) = atan((1-R_alpha)*tan(Fi(k)));    % приведенная широта
end %перевод в радианы
k = 1;
for N_RAZ = 1:N_point - 2
    if Fi(k+1) < Fi(k+2) && Lam(k+1) < Lam(k+2)
        roll_route(N_RAZ) =  int8(-1);
    else
        roll_route(N_RAZ) =  int8(1);
    end
    k = k + 1;
end %направление крена
tr_H = flight_mission(3,1);
%Решение обратной геодезической задачи на эллипсоиде
for k = 1:N_point-1
    l(k) = Lam(k+1) - Lam(k);
    w(k) = l(k);                % разность долгот в первом приближении
end
for k = 1:N_point-1
    i = 1;
    while i <= 6
        cos_sigma_R(k) = sin(U(k))*sin(U(k+1)) + cos(U(k))*cos(U(k+1))*cos(w(k));
        sin_sigma_R(k) = sqrt(1 - cos_sigma_R(k).^2);
        sin_A_0(k) = cos(U(k))*cos(U(k+1))*sin(w(k))/sin_sigma_R(k);
        cos_2sigma_R_m(k) = cos_sigma_R(k) - 2*sin(U(k))*sin(U(k+1))/(1 - sin_A_0(k).^2);
        %sigma_R_m(k) = sqrt(acos(cos_2sigma_R_m(k)));
        C(k) = (R_alpha/16) * (1 - sin_A_0(k).^2) * (4+R_alpha*(4-3*(1 - sin_A_0(k).^2)));
        w(k) = l(k) + (1-C(k))*R_alpha*sin_A_0(k) * (acos(cos_sigma_R(k)) + C(k)*sin_sigma_R(k) * (cos_2sigma_R_m(k) + C(k)* cos_sigma_R(k)*( -1 + 2*cos_2sigma_R_m(k).^2)));
        i = i + 1;
    end
    k2(k) = R_e12*(1 - sin_A_0(k).^2);
    A(k) = 1 + (k2(k)/256)*(64 + k2(k)*( - 12 + 5*k2(k)));
    B(k) = (k2(k)/512)*(128 + k2(k)*( - 64 + 37*k2(k)));
    del_sigma(k) = B(k)*sin_sigma_R(k) * (cos_2sigma_R_m(k) + (B(k)/4)*cos_sigma_R(k)*( -1 + (2*(cos_2sigma_R_m(k).^2))));
    s(k) = (R_b*A(k) + tr_H) *(acos(cos_sigma_R(k)) - del_sigma(k));  %длины линий между точками
    course_R(k) = atan((cos(U(k+1))*sin(w(k)))/(cos(U(k))*sin(U(k+1)) - sin(U(k))*cos(U(k+1))*cos(w(k))));
    course(k) = course_R(k);
    %уточнение квадранта для угла курса
    if Fi(k) > Fi(k+1) && Lam(k) <= Lam(k+1)
        course(k) = pi + course_R(k);
    end
    if Fi(k) > Fi(k+1) && Lam(k) > Lam(k+1)
        course(k) = course_R(k) - pi;
    end
    PSI(k) = course(k)*180/pi;
    %Ts(k) = (s(k) - R_turn_roll)/Vp;    %время движения по ЛЗП
end

dd_roll = d_roll*dT;    %шаг изменения крена
t_roll = roll_turn/d_roll;  %время поворота по крену

for k = 1:N_point-2
    UR(k) = abs(course_R(k+1) - course_R(k));
    LUR(k) = R_turn_roll*tan(UR(k)/2);  %расчет линий упрежденного разворота
    T_LZP(k) = (s(k) - LUR(k))/tr_Vp;      %время движения по ЛЗП
    HORD(k) = sqrt(2*LUR(k).^2-2*LUR(k).^2 *cos(UR(k)));% Рассчет хорды окружности разворота
    aaa(k) = HORD(k)/(2 * R_turn_roll); %HORD(k)/(2*R_turn_roll);
    ALPHA(k) = 2*asin(aaa(k)); %Угол вписанной окружности разворота, лежащей на хорде
    Lrz(k) = ALPHA(k)*R_turn_roll; %Длина дуги разворота
    T_rz(k) = round((Lrz(k)/tr_Vp)); %Время разворота
    t1_roll(k) =  t_roll;
    t2_roll(k) = T_rz(k) - t_roll;  %время участков крена
    d_course_R(k) = (ALPHA(k))/T_rz(k); %угловая скорость  курса
    dd_course(k) = d_course_R(k)*dT; %Шаг изменения курса
    if Fi(k+1) < Fi(k+2) && Lam(k+1) < Lam(k+2)
        dd_course(k) = -dd_course(k);
    end
    N = T_rz(k)/dT; %Кол-во шагов цикла
end
T_LZP(k+1) = s(k+1)/tr_Vp;      %время движения по последней ЛЗП
T_mod = sum(T_LZP) + sum(T_rz);
N = T_mod/dT; %Кол-во шагов цикла
i = 1;
k = 1;
t = 0;
tr_fi(1) = U(1);
tr_la(1) = Lam(1);
N_P = 1;    %номер этапа траектории
N_LZP = 1;
N_RAZ = 1;
Ts = 0;
%вручную поправляем направление измерения курса:
course_kvadrant = [1; 1; 1; 1; -1; -1; -1];
%course_kvadrant = [1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1];
T_S = 1;
tr_course(1) = course(1);
while N_P <= N_point+6  %10
    while t <= T_LZP(N_LZP)
        tr_course(i) = course(N_LZP);
        tr_V_E(i) = tr_Vp*sin(tr_course(i));
        tr_V_N(i) = tr_Vp*cos(tr_course(i));
        tr_V_H(i) = 0;
        tr_H(i+1) = tr_V_H(k)*dT+tr_H(k);
        %Rk = R_a/sqrt(1 + R_e2*sin(tr_fi(i)));
        Rk = R_rz;
        tr_fi(i+1) = (tr_V_N(i)*dT)/(Rk+tr_H(k))+tr_fi(i);
        tr_la(i+1) = (tr_V_E(i)*dT)/(Rk+tr_H(k))+tr_la(i);
        tr_roll(i) = 0;
        tr_pitch(i) = 0;
        t = t + dT;
        i = i + 1;
        Ts = Ts + dT;
    end
    N_LZP = N_LZP + 1;
    k = 1;
    t = 0;
    N_P = N_P + 1;
    course_R = course(N_RAZ);
    if rem(N_P,2) == 0 && N_P < N_point + 6 %10
        while t <= T_rz(N_RAZ) + 0.25
            course_R = course_R + dd_course(N_RAZ) * course_kvadrant(N_RAZ);
            tr_course(i) = course_R;
            tr_V_E(i) = tr_Vp*sin(tr_course(i));
            tr_V_N(i) = tr_Vp*cos(tr_course(i));
            tr_V_H(i) = 0;
            tr_H(i+1) = tr_V_H(k)*dT+tr_H(k);
            Rk = R_a/sqrt(1 + R_e2*sin(tr_fi(i)));
            %Rk = R_rz;
            tr_fi(i+1) = (tr_V_N(i)*dT)/(Rk+tr_H(k))+tr_fi(i);
            tr_la(i+1) = (tr_V_E(i)*dT)/(Rk+tr_H(k))+tr_la(i);
% % если крен меняется:
%             if t < t1_roll(N_RAZ) 
%                 if roll_route(N_RAZ) == -1
%                     tr_roll(i) = tr_roll(i-1) - dd_roll;
%                 else
%                     tr_roll(i) = tr_roll(i-1) + dd_roll;
%                 end
%             else
%                 if t > t2_roll(N_RAZ) + 0.25
%                     if roll_route(N_RAZ) == -1
%                         tr_roll(i) = tr_roll(i-1) + dd_roll;
%                     else
%                         tr_roll(i) = tr_roll(i-1) - dd_roll;
%                     end
%                 else
%                     tr_roll(i) = tr_roll(i-1);
%                 end
%             end
            tr_roll(i) = 0;
            tr_pitch(i) = 0;
            t = t + dT;
            i = i + 1;
            Ts = Ts + dT;
        end
        N_RAZ = N_RAZ + 1;
        N_P = N_P + 1;
    end
end
% for i = 1:1:Ts
%     if tr_course(i) < 0
%         tr_course(i) = pi - tr_course(i);
%     end
% end
    
fi_grad = tr_fi*180/pi;
la_grad = tr_la*180/pi;
% %course = tr_course*180/pi;
% %roll = tr_roll*180/pi;
plot(la_grad, fi_grad)
% grid on
% xlabel('Долгота, град');
% ylabel('Широта, град');

%plot(roll)

% %начальные условия модели погрешностей БИНС:
% %ошибки определения координат БИНС
% x1 = 1;
% x2 = 1;
% %ошибка определения высоты РВ
% del_h = 0.2;
% %ошибки в определении скорости изменения координат БИНС
% x4 = 0.05;
% x5 = 0.05;
% %ошибка определения вертикальной скорости РВ
% del_Vh = 0.01;
% %ошибки ориентации базового трехгранника БИНС
% alpha_BINS = 0.01*pi/(180);
% betta_BINS = 0.01*pi/(180);
% gamma_BINS = 0.01*pi/(180);
% %проекции постоянных составляющих погрешностей ДУС на оси связанной СК
% del_Omega_x = 0.0005*pi/(180*3600); %сделать минус 2 порядка
% del_Omega_y = 0.0005*pi/(180*3600);
% del_Omega_z = 0.0005*pi/(180*3600);
% %ошибки масштабных коэффициентов ДУС
% del_kOmega_x = 0.0004; %сделать минус 2 порядка
% del_kOmega_y = 0.0004;
% del_kOmega_z = 0.0004;
% %постоянные составляющие ошибок акселерометров в связанной СК
% del_n_x = 0.0002;
% del_n_y = 0.0002;
% del_n_z = 0.0002;
% %ошибки масштабных коэффициентов акселерометров
% del_kn_x = 0.0001;
% del_kn_y = 0.0001;
% del_kn_z = 0.0001;
% %СКО шумовых составляющих погрешностей ДУС в связанной СК
% SKO_del_Omega_x = 0.005*pi/(180*3600);
% SKO_del_Omega_y = 0.005*pi/(180*3600);
% SKO_del_Omega_z = 0.005*pi/(180*3600);
% %СКО шумовых составляющих погрешностей акселерометров в связанной СК
% SKO_del_n_x = 0.001;
% SKO_del_n_y = 0.001;
% SKO_del_n_z = 0.001;

% load('sav_traj_1_ins.mat')
% tr_fi = pos_lat;
% tr_la = pos_lon;
% tr_H = pos_alt;
% tr_roll = roll;
% tr_roll(:) = 0;
% tr_course = heading;
% tr_pitch = pitch;
% tr_pitch(:) = 0;
% tr_V_E = vel_lon;
% tr_V_N = vel_lat;
% tr_V_H = vel_alt;
% tr_V_H(:) = 0;
% Ts = size(time);
% Ts = Ts(2);
% t = time;
% tr_course(1:985) = 0;
% tr_course(4844:5735) = 2 * pi;

% %начальные условия модели погрешностей БИНС (навигационный):
% %ошибки определения координат БИНС
% x1 = 10;
% x2 = 10;
% %ошибка определения высоты РВ
% del_h = 0;
% %ошибки в определении скорости изменения координат БИНС
% x4 = 0.1;
% x5 = 0.1;
% %ошибка определения вертикальной скорости РВ
% del_Vh = 0.0;
% %ошибки ориентации базового трехгранника БИНС
% alpha_BINS = 0.1*pi/(180);
% betta_BINS = 0.1*pi/(180);
% gamma_BINS = 0.1*pi/(180);
% % alpha_BINS = 1*pi/(180);
% % betta_BINS = 1*pi/(180);
% % gamma_BINS = 1*pi/(180);
% %проекции постоянных составляющих погрешностей ДУС на оси связанной СК
% % del_Omega_x = 0.001*pi/(180*3600);
% % del_Omega_y = 0.001*pi/(180*3600);
% % del_Omega_z = 0.001*pi/(180*3600);
% del_Omega_x = 0.001*pi/(180*3600);
% del_Omega_y = 0.001*pi/(180*3600);
% del_Omega_z = 0.001*pi/(180*3600);
% %ошибки масштабных коэффициентов ДУС
% del_kOmega_x = 0.002;
% del_kOmega_y = 0.002;
% del_kOmega_z = 0.002;
% % del_kOmega_x = 0.2;
% % del_kOmega_y = 0.2;
% % del_kOmega_z = 0.2;
% %постоянные составляющие ошибок акселерометров в связанной СК
% del_n_x = 0.002;
% del_n_y = 0.002;
% del_n_z = 0.002;
% % del_n_x = 0.02;
% % del_n_y = 0.02;
% % del_n_z = 0.02;
% %ошибки масштабных коэффициентов акселерометров
% del_kn_x = 0.002;
% del_kn_y = 0.002;
% del_kn_z = 0.002;
% %СКО шумовых составляющих погрешностей ДУС в связанной СК
% SKO_del_Omega_x = 0.001*pi/(180*3600);
% SKO_del_Omega_y = 0.001*pi/(180*3600);
% SKO_del_Omega_z = 0.001*pi/(180*3600);
% % SKO_del_Omega_x = 1*pi/(180*3600);
% % SKO_del_Omega_y = 1*pi/(180*3600);
% % SKO_del_Omega_z = 1*pi/(180*3600);
% %СКО шумовых составляющих погрешностей акселерометров в связанной СК
% SKO_del_n_x = 0.05;
% SKO_del_n_y = 0.05;
% SKO_del_n_z = 0.05;
% %начальные условия модели погрешностей БИНС (навигационный):
%ошибки определения координат БИНС
x1 = 10;
x2 = 10;
%ошибка определения высоты РВ
del_h = 0;
%ошибки в определении скорости изменения координат БИНС
x4 = 0.1;
x5 = 0.1;
%ошибка определения вертикальной скорости РВ
del_Vh = 0.0;
%ошибки ориентации базового трехгранника БИНС
alpha_BINS = 0.001*pi/(180);
betta_BINS = 0.001*pi/(180);
gamma_BINS = 0.005*pi/(180);
%проекции постоянных составляющих погрешностей ДУС на оси связанной СК
del_Omega_x = 0.0001*pi/(180*3600);
del_Omega_y = 0.0001*pi/(180*3600);
del_Omega_z = 0.0001*pi/(180*3600);
%ошибки масштабных коэффициентов ДУС
del_kOmega_x = 0.002;
del_kOmega_y = 0.002;
del_kOmega_z = 0.002;
%постоянные составляющие ошибок акселерометров в связанной СК
del_n_x = 0.002;
del_n_y = 0.002;
del_n_z = 0.002;
%ошибки масштабных коэффициентов акселерометров
del_kn_x = 0.002;
del_kn_y = 0.002;
del_kn_z = 0.002;
%СКО шумовых соcтавляющих погрешностей ДУС в связанной СК
SKO_del_Omega_x = 0.001*pi/(180*3600);
SKO_del_Omega_y = 0.001*pi/(180*3600);
SKO_del_Omega_z = 0.001*pi/(180*3600);
%СКО шумовых составляющих погрешностей акселерометров в связанной СК
SKO_del_n_x = 0.005;
SKO_del_n_y = 0.005;
SKO_del_n_z = 0.005;
%век
% Параметры модели погрешностей псевдодальностей
c = 299792458;  %скорость света в вакууме
SKO_p_del_T = 3;  %СКО ошибки псевдодальнотсей, вызванной ошибкой часов НКА 
SKO_p_del_t = 0.01; %СКО ошибки псевдоскоростей, вызванной ошибкой часов приемника
mean_p_ion = 1.3; %среднее значение ошибки псевдодальностей, вызванной ионосферной задержкой
mean_p_trop = 0.3;%среднее значение ошибки тропосферной, вызванной тропосферной задержкой
SKO_p_mnogoluch = 0.5;  %СКО ошибки псевдодальнотсей, вызванной многолучевостью (пока выключим)
SKO_p_noise = 0.5;  %СКО ошибки псевдодальнотсей, вызванной шумами приемника
al_gp_max = 15; %максимальное кол-во теоретически доступных спутников (выделение памяти под все сигналы НКА)

sigma_gnss0 = SKO_p_del_T + SKO_p_del_t + mean_p_ion + mean_p_trop + SKO_p_mnogoluch + SKO_p_noise;
sigma_error_DUS0 = [SKO_del_Omega_x SKO_del_Omega_y SKO_del_Omega_z];
sigma_error_acs0 = [SKO_del_n_x SKO_del_n_y SKO_del_n_z];
sigma_error_D_p (1:31) = SKO_p_mnogoluch; %енадо разобратсья 
sigma_error_D_p1(1:31) = SKO_p_noise;

%Параметры модели погрешностей ОЭС

SKO_o0 = 5;

X0 = [x1 x2 del_h x4 x5 del_Vh alpha_BINS betta_BINS gamma_BINS del_Omega_x del_Omega_y del_Omega_z del_kOmega_x del_kOmega_y del_kOmega_z del_n_x del_n_y del_n_z del_kn_x del_kn_y del_kn_z];
%%
%X0(2:21) = 0;
%%
%X0_19 = [x1 x2 x4 x5 alpha_BINS betta_BINS gamma_BINS del_Omega_x del_Omega_y del_Omega_z del_kOmega_x del_kOmega_y del_kOmega_z del_n_x del_n_y del_n_z del_kn_x del_kn_y del_kn_z];
X0_b_g = [x1 x2 del_h x4 x5 del_Vh alpha_BINS betta_BINS gamma_BINS del_Omega_x del_Omega_y del_Omega_z del_kOmega_x del_kOmega_y del_kOmega_z del_n_x del_n_y del_n_z del_kn_x del_kn_y del_kn_z SKO_p_del_T SKO_p_del_t];

disp('X0=')
%X0_b_o0 = [x1 x2 del_h x4 x5 del_Vh alpha_BINS betta_BINS gamma_BINS del_Omega_x del_Omega_y del_Omega_z del_kOmega_x del_kOmega_y del_kOmega_z del_n_x del_n_y del_n_z del_kn_x del_kn_y del_kn_z dR11_0 dR12_0 dR13_0 dR21_0 dR22_0 dR23_0 dR31_0 dR32_0 dR33_0]

%X0_b_g(1:21) = X0;
X0 = X0';
X0_b_g = X0_b_g * 0;

P_b = zeros(21, 21); 

P0_b_g = zeros(23,23);

%начальные значения элементов матрицы ковариации P_b (СКО, в квадрат
%возводится ниже)
%x1 x2 x3
P_b(1,1) = 1;
P_b(2,2) = 1;
P_b(3,3) = 0.001;
% x4 x5 x6
P_b(4,4) = 0.1;
P_b(5,5) = 0.1;
P_b(6,6) = 0.001;
%x7 x8 x9
P_b(7,7) = 0.005 * pi/180;
P_b(8,8) = 0.005 * pi/180;
P_b(9,9) = 0.01 * pi/180;

P_b(10,10) = 0.05*pi/(180*3600);
P_b(11,11) = 0.05*pi/(180*3600);
P_b(12,12) = 0.05*pi/(180*3600);
P_b(13,13) = 5.0000e-05;
P_b(14,14) = 5.0000e-05;
P_b(15,15) = 5.0000e-05;
P_b(16,16) = 1.0000e-03;
P_b(17,17) = 1.0000e-03;
P_b(18,18) = 1.0000e-03;
P_b(19,19) = 1.0000e-05;
P_b(20,20) = 1.0000e-05;
P_b(21,21) = 1.0000e-05;

%матрица ковариации БИНС + ОЭС

CP_M_max = 20;
p_max = CP_M_max;
P0_b_o = zeros(21 + 3 * p_max,21 + 3 * p_max);
P0_b_o(1:21,1:21) = P_b;

for i = 1:3:3 * p_max
    P0_b_o(21 + i,21 + i) = 0;
    P0_b_o(22 + i,22 + i) = 0;
    P0_b_o(23 + i,23 + i) = 0;
end

X0_b_o0 = zeros(21 + 3 *p_max,1);
%матрица ковариации БИНС + ГНСС
P0_b_g(1:21,1:21) = P_b;
P0_b_g(22,22) = SKO_p_del_T;
P0_b_g(23,23) = SKO_p_del_t;

for i = 1:1:(21 + 3 * p_max)
    P0_b_o(i,i) = P0_b_o(i,i) .^ 2;
end
    
for i = 1:1:23
    P0_b_g(i,i) = P0_b_g(i,i) .^ 2;
end
    
W = [SKO_del_Omega_x SKO_del_Omega_y SKO_del_Omega_z SKO_del_n_x SKO_del_n_y SKO_del_n_x];
disp('Q=')
W = W';

t = zeros(i, 1);
k = 1;
for tt = 0:dT:Ts
    t(k) = tt;
    k = k + 1;
end

clearvars -EXCEPT tr* R* dT Ts X0* W t i al* SKO* mean* P0* P_b sigma_* dR* CP_M_max