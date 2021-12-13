function [X_k_list, X_full_list, X_m_list, Pk_list, Zk_list] = Kalman(lam,fi, psi_list,gam_list, V_e, V_n, R1, R2, aw_e, aw_n, aw_h, alfa, betta, gamma, acc_E, acc_N, acc_H, w_x, w_y, w_z,n_X, n_Y,n_Z,omega_Z_dot)
X_out=zeros(19,1);
dt = 1;
w0 = 1.25e-3;
V_H = 0;
q=0.00346775; % отношение центробежной силы, возникающей вследствие вращени€ «емли, к силе т€жести на экваторе
bet=0.0053171;
bet1=71*10^-7;
psi_error=0.25*pi/180;%курс
gamma_error=0.03*pi/180;%крен
tetta_error=0.03*pi/180;%тангаж
tetta = 0;
a=6378245;% больша€ полуось
b=6356863; %мала€ полуось
e=sqrt(a^2-b^2)/a;
u=(pi*15/3600)/180;
g=9.78049;
H = 1000;

gyro_zero_error=0.001*pi/180;
gyro_noise=gyro_zero_error*0.01;
gyro_koef_error=2*10^-5;

ac_zero_error=g*6*(10^-6);
ac_noise=ac_zero_error*0.001;
ac_koef_error=3*10^-6;


for i=2:1:length(lam)-1
    roll=gam_list(i-1);
    ro1 = R1(i-1);
    ro2 = R2(i-1);
   
    % изменение проекций скорости в нормальной системе координат
    if i==2
        d_V_e=0;
        d_V_n=0;
    else
        d_V_e=(V_e(i-1)-V_e(i-2));
        d_V_n=(V_n(i-1)-V_n(i-2));
    end
   
    
    %абсолютные угловые скорости
    absOmega_e=aw_e(i-1);
    absOmega_n=aw_n(i-1);
    absOmega_H=aw_h(i-1);
    
    % ошибки бинс
    if i==2
        x1=15;
        x2=15;
        x3=0.5;
        x4=0.5;
    else
        x1=x1+X_out(1);
        x2=x2+X_out(2);
        x3=x3+X_out(3);
        x4=x4+X_out(4);
    end

    

    alfa2=alfa(i-1);
    betta2=betta(i-1);
    gamma2=gamma(i-1);


    % кажущиес€ ускорени€ в нормальной ск
    n_e=acc_E(i-1);
    n_n=acc_N(i-1);
    n_H=acc_H(i-1);
    
    % матрица направл€ющих косинусов
    C=[-sin(psi_list(i-1))*cos(tetta), sin(psi_list(i-1))*sin(tetta)*cos(roll)+cos(psi_list(i-1))*sin(roll), -sin(psi_list(i-1))*sin(tetta)*sin(roll)+cos(psi_list(i-1))*cos(roll);
        cos(psi_list(i-1))*cos(tetta), -cos(psi_list(i-1))*sin(tetta)*cos(roll)+sin(psi_list(i-1))*sin(roll), cos(psi_list(i-1))*sin(tetta)*cos(roll)+sin(psi_list(i-1))*cos(roll);
        sin(tetta), cos(tetta)*cos(roll), -cos(tetta)*sin(roll)];
    omega_x=w_x(i-1);
    omega_y=w_y(i-1);
    omega_z=w_z(i-1);
    
    n_x=n_X(i-1);
    n_y=n_Y(i-1);
    n_z=n_Z(i-1);
    % проекци€ скорости изменени€ абсолютной угловой скорости вращени€ на вертикаль места
    dot_omega_H=omega_Z_dot(i-1);
    
    %  матрица динамики системы полна€
    F=zeros(19);
    F(1,3)=1;
    F(2,4)=1;
    F(3,:)=[-w0^2+absOmega_n^2+absOmega_H^2,dot_omega_H-absOmega_e*absOmega_n,0,2*absOmega_H,0,n_H,0,0,0,0,0,0,0,C(1,1),C(1,2),C(1,3),n_x*C(1,1),n_y*C(1,2),n_z*C(1,3)];
    F(4,:)=[-(dot_omega_H-absOmega_e*absOmega_n),-w0^2+absOmega_e^2+absOmega_H^2,-2*absOmega_H,0,-n_H,0,n_e,0,0,0,0,0,0,C(2,1),C(2,2),C(2,3),n_x*C(2,1),n_y*C(2,2),n_z*C(2,3)];
    F(5,:)=[0,0,0,0,0,absOmega_H,-absOmega_n,C(1,1),C(1,2),C(1,3),omega_x*C(1,1),omega_y*C(1,2),omega_z*C(1,3),0,0,0,0,0,0];
    F(6,:)=[0,0,0,0,-absOmega_H,0,absOmega_e,C(2,1),C(2,2),C(2,3),omega_x*C(2,1),omega_y*C(2,2),omega_z*C(2,3),0,0,0,0,0,0];
    F(7,:)=[0,0,0,0,absOmega_H,-absOmega_e,0,C(3,1),C(3,2),C(3,3),omega_x*C(3,1),omega_y*C(3,2),omega_z*C(3,3),0,0,0,0,0,0];
    F_list(:,:,i-1)=F;
        
    % матрица шумов системы
    G=zeros(19,19);
    G(3,:)=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,C(1,1),C(1,2),C(1,3)];
    G(4,:)=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,C(2,1),C(2,2),C(2,3)];
    G(5,:)=[0,0,0,0,0,0,0,0,0,0,0,0,0,C(1,1),C(1,2),C(1,3),0,0,0];
    G(6,:)=[0,0,0,0,0,0,0,0,0,0,0,0,0,C(2,1),C(2,2),C(2,3),0,0,0];
    G(7,:)=[0,0,0,0,0,0,0,0,0,0,0,0,0,C(3,1),C(3,2),C(3,3),0,0,0];
    G_list(:,:,i-1)=G;
    % вектора

    X_full=[x1; x2; x3; x4; alfa2; betta2; gamma2; gyro_zero_error; gyro_zero_error; gyro_zero_error; gyro_koef_error; gyro_koef_error; gyro_koef_error; ac_zero_error; ac_zero_error; ac_zero_error; ac_koef_error; ac_koef_error; ac_koef_error];

    W=[0,0,0,0,0,0,0,0,0,0,0,0,0,gyro_noise, gyro_noise, gyro_noise, ac_noise, ac_noise, ac_noise]';
    
    % решение
    
    X_out=(F*X_full+G*randn*W)*dt;
    X_out_list(:,i-1)=X_out;
    X_full_list(:,i-1)=X_full;
    

    %% модель
    
    Hk=zeros(4,19);% матрица св€зи вектора сост и вектора измерений
    Hk(1,1)=1/(ro2*cos(fi(i-1)));
    Hk(2,2)=1/ro1;
    Hk(3,:)=[-(V_H/ro2+absOmega_e*tan(fi(i-1))),-absOmega_H,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
    Hk(4,:)=[-V_H/ro1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
    H_list(:,:,i-1)=H;
    noise_lam=5/(ro2*cos(fi(i-1)));% шумова€ составл€юща€ долготного канала
    noise_fi=5/ro1;% шумова€ составл€юща€ широтного канала

    noise_V_e=.05;% шумова€ составл€юща€ определени€ восточной проекции скорости
    noise_V_n=.05;% шумова€ составл€юща€ определени€ северной проекции скорости
    VV=[noise_lam;noise_fi;noise_V_e;noise_V_n];% вектор шумов измерений снс
    

    Zk=Hk*X_full+VV;% вектор измерений системы
    Zk_list(:,i-1)=Zk;
    
    
    %% ќ‘ %%%%%%%%%%%%%%ќ‘ %%%
    % ковариаци€ шума измерени€
    Rk=diag(VV);
    Rk=Rk.^2;
    Rk=Rk/dt;
    % ковариаци€ шума процесса
    Qk=diag(W);
    Qk=Qk.^2;
    Qk=Qk/dt;
    if i==2
        Pk_o=diag(X_full);
        Pk_o=Pk_o.^2;
        X_m=X_full;
    end
    a = eye(19)+F*dt;
    b = (F*dt).^2;
    c = b / 2;
    Fk=(a+c); % матрица динамики системы в дискретном видел
    
    Gk=(eye(19)+F*dt/2+((F*dt).^2)/6)*G*dt; % матрица шумов системы в дискретном виде 
    
    
    Sk=Fk*Pk_o*Fk'+Gk*Qk*Gk';% априорна€ матрица ковариации (оценка)
    
    K=Sk*Hk'*(Hk*Sk*Hk'+Rk)^-1;% матрица коэффициентов усилени€
    
    Pk_o=(eye(19)-K*Hk)*Sk;% опастариорна€ матрица ковариации (прогноз)
    a1 = Fk*X_m;
    b1 = Hk*Fk*X_m;
    c1 = Zk - b1;
    d1 = K * c1;
    X_m=a1 + d1;% нахождение оценки
    
    X_k=X_full-X_m;% вектор отфильтрованных данных
    % формирование массивов дл€ построени€ графиков отфильтрованных
    % выходных величин и — ќ
    X_k_list(:,i-1)=X_k;
    X_m_list(:,i-1)=X_m;
    Pk_list(1,i-1)=sqrt(Pk_o(1,1));
    Pk_list(2,i-1)=sqrt(Pk_o(2,2));
    Pk_list(3,i-1)=sqrt(Pk_o(3,3));
    Pk_list(4,i-1)=sqrt(Pk_o(4,4));
    
    
    
end

end

