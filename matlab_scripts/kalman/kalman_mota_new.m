clc
clear
close all
X_out=zeros(19,1);
%%%��������� ������%%%
%%%��������� ������%%%
%%%��������� ������%%%
Rz=6371000; % ������ �����
t=12000;% ����� ������
fi_o = [55, 56.58, 56.46, 56.96];%������
lam_o = [37, 37.5, 38, 38.5];%�������
H = [1000, 1000, 1000, 1000]; %������ �� �������� ������� ������� ���:
H = 1000;
V_H=0;
tetta=0;%������ �� �������� ���� ������� ����������
dt = 1;%���
gam = pi*46/180;%���� ��
g=9.78049; %��������� ���� ������� �� ��������
u=(pi*15/3600)/180;
w0=sqrt(g/Rz);%������� ������
%% ��������� ���������� %%
a=6378245;% ������� �������
b=6356863; %����� �������
e=sqrt(a^2-b^2)/a; %  ��������������
%%
q=0.00346775; % ��������� ������������ ����, ����������� ���������� �������� �����, � ���� ������� �� ��������
bet=0.0053171;
bet1=71*10^-7;
%% ������ ��������� ��������
psi_error=0.25*pi/180;%����
gamma_error=0.03*pi/180;%����
tetta_error=0.03*pi/180;%������

%% ��������
i=1;
k=1;
j=1;
%% ��������� ��������
fi_o=fi_o.*pi/180;
lam_o=lam_o.*pi/180;

for j = 2:1:4
    w(j-1)=lam_o(j)-lam_o(j-1);%��������� �������
    sigma(j-1)=acos(sin(fi_o(j-1))*sin(fi_o(j))+cos(fi_o(j-1))*cos(fi_o(j))*cos(w(j-1)));%����� ���������� � �������� ����� ���(j-1) � ���(j)
    D(j-1)=Rz*sigma(j-1); %�������� �� �������� � ����� ����� ����������
    %���� �����(�������� ������������� ������ �� �����)
    psi(j-1)=atan2(cos(fi_o(j))*sin(w(j-1)),cos(fi_o(j-1))*sin(fi_o(j))-sin(fi_o(j-1))*cos(fi_o(j))*cos(w(j-1)));
    %���� �������� ����� ����� = [0:2*pi]
    if (psi(j-1) <= 0)
        psi(j-1) = psi(j-1) + 2*pi;
    end
    if (psi(j-1) > 2*pi)
        psi(j-1) = psi(j-1) - 2*pi;
    end
    
end
sum_distance = sum(D);%��������� ���������� �� �����
dist=sum_distance;
V = sum_distance/t;%�������� �������� (��� ���������)
%������ ��������� ��������
fi(1) = fi_o(1);
lam(1) = lam_o(1);
gam_list(i)=0;
psi_list(i)=-psi(1)+2*pi;

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
for k = 1:3
    %%%�������� ��������� ��� ����������� ������%%%
    %%%�������� ��������� ��� ����������� ������%%%
    %%%�������� ��������� ��� ����������� ������%%%
    if k~=1
        
        w(k)=lam_o(k+1)-lam(i);%��������� �������
        sigma(k)=acos(sin(fi(i))*sin(fi_o(k+1))+cos(fi(i))*cos(fi_o(k+1))*cos(w(k)));%����� ���������� � �������� ����� ���(k) � ���(k+1)
        D(k)=Rz*sigma(k); %�������� �� �������� � ����� ����� ����������
        %���� �����(�������� ������������� ������ �� �����)
        psi(k)=atan2(cos(fi_o(k+1))*sin(w(k)),cos(fi(i))*sin(fi_o(k+1))-sin(fi(i))*cos(fi_o(k+1))*cos(w(k)));
        %���� �������� ����� ����� = [0:2pi]
        if (psi(k) <= 0)
            psi(k) = psi(k) + 2*pi;
        end
        if (psi(k) > 2*pi)
            psi(k) = psi(k) - 2*pi;
        end
        
        sum_distance = sum(D);%��������� ���������� �� �����
        V = sum_distance/t;%�������� �������� (��� ���������)
    end
    %�������� ���������� ��������
    %������������ ������������ �����������
    %��������������� ������������
    Ve = V*sin(psi(k));%������
    Vn = V*cos(psi(k));%�����
    %������� ��������
    w_E = Vn/Rz;
    w_N = Ve/Rz;
    
    %%%������� ���������� ���%%%
    %%%������� ���������� ���%%%
    %%%������� ���������� ���%%%
    if (k ~= 3)
        UR = psi(k+1) - psi(k);
        if UR<0
            gam=-gam;
        end
        Rr = V^2 / (9.81 * tan(gam));%������ ���������
        Tr = Rr*UR/V;%����� ���������
        lur_dist = Rr*tan(0.5*UR);%���������� ���
        Tr_list(k)=Tr;
    end
    ppm_dist = Rz*sigma(k);%����� ���� �� ���(i-1) �� ���(i)
    ppm_dist_past = ppm_dist+1;
    
    %%%���������� ���������� ���� ���� ����� �������� ����������%%%
    %%%���������� ���������� ���� ���� ����� �������� ����������%%%
    %%%���������� ���������� ���� ���� ����� �������� ����������%%%
    while (lur_dist < ppm_dist)&&(ppm_dist_past > ppm_dist)
        i = i + 1;
        gam_list(i)=0;
        ppm_dist_past = ppm_dist;
        %���������� ���������
        dfi = w_E;
        dlam = w_N/cos(fi(i-1));
        %���������� ��������� ����� dt
        fi(i) = fi(i-1) + dfi*dt;
        lam(i) = lam(i-1) + dlam*dt;
        %�������� ����� ���������� c ������ ����������� ����������
        sig = acos(sin(fi(i))*sin(fi_o(k+1))+cos(fi(i))*cos(fi_o(k+1))*cos(lam_o(k+1) - lam(i)));
        ppm_dist = Rz*sig;
        ppm_dist_list(i) = ppm_dist;
        psi_list(i)=-psi(k)+2*pi;
        
    end
    dPsi = dt*UR/Tr;%���������� ���� ����� �� dt �� ����� ���
    %%%���������� ���������� ��� ��������� ���� ����� (���)%%%
    %%%���������� ���������� ��� ��������� ���� ����� (���)%%%
    %%%���������� ���������� ��� ��������� ���� ����� (���)%%%
    turn_start(k)=i;
    if (k ~= 3)
        for j = 0:dt:Tr
            
            i = i + 1;
            gam_list(i)=gam;
            psi(k) = psi(k) + dPsi;%��������� ���� ����� �� ����� dt
            Ve = V*sin(psi(k));
            Vn = V*cos(psi(k));
            w_E = Vn/Rz;
            w_N = Ve/Rz;
            dfi = w_E;
            dlam = w_N/cos(fi(i-1));
            fi(i) = fi(i-1) + dfi*dt;
            lam(i) = lam(i-1) + dlam*dt;
            psi_list(i)=-psi(k)+2*pi;
            
        end
    end
    turn_end(k)=i;
end
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
i=1;
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%% Solving

for i=2:1:length(lam)-1
    gamma=gam_list(i-1);
    % ������ �������� (����������������
    gyro_zero_error=0.001*pi/180;
    gyro_noise=gyro_zero_error*0.01;
    gyro_koef_error=2*10^-5;

    ac_zero_error=g*60*(10^-6);
    ac_noise=ac_zero_error*0.01;
    ac_koef_error=3*10^-6;
    
    %% ��������
    V_e(i-1)=V*cos(psi_list(i-1));
    V_n(i-1)=V*sin(psi_list(i-1));
    %% ��������� �������� �������� � ���������� ������� ���������
    if i==2
        d_V_e=0;
        d_V_n=0;
    else
        d_V_e=(V_e(i-1)-V_e(i-2));
        d_V_n=(V_n(i-1)-V_n(i-2));
    end
    %�������� ������� �������� �������� �����
    u_n=u*cos(fi(i-1));
    u_H=u*sin(fi(i-1));
    %������� ��������, ��������������, ��������������� � ����������������� ��� �������
    ro1=(a*(1-e^2)/(sqrt(1-e^2*sin(fi(i-1))^2)^3))+H;
    ro2=(a/sqrt(1-e^2*sin(fi(i-1))^2))+H;
    %������������� ������� ��������
    omega_e_1=-V_n(i-1)/ro1;
    omega_n_1=V_e(i-1)/ro2;
    omega_H_1=tan(fi(i-1))*(V_e(i-1)/ro2);
    
    %���������� ������� ��������
    omega_e=omega_e_1;
    omega_n=omega_n_1+u_n;
    omega_H=omega_H_1+u_H;
    
    % ������ ����
    if i==2
        x1=6;
        x2=6;
        x3=0.05;
        x4=0.05;
    else
        x1=x1+X_out(1);
        x2=x2+X_out(2);
        x3=x3+X_out(3);
        x4=x4+X_out(4);
    end

    
    %% ������ ���������� ������������ ���� ������������ ������������ ��������

    turn=[sin(psi_list(i-1))*tan(tetta), cos(psi_list(i-1))*tan(tetta), -1;
        cos(psi_list(i-1)), -sin(psi_list(i-1)), 0;
        sin(psi_list(i-1))/cos(tetta), cos(psi_list(i-1))/cos(tetta), 0];
    eler_er=[psi_error;tetta_error;gamma_error];
    or_er=turn^-1*eler_er;
    
    alfa_er=or_er(1)+X_out(5);
    betta_er=or_er(2)+X_out(6);
    gamma_er=or_er(3)+X_out(7);
    
    alfa1=-x2/ro1;
    betta1=x1/ro2;
    gamma1=x1*tan(fi(i-1))/ro2;
    
    alfa2=alfa_er+alfa1;
    betta2=betta_er+betta1;
    gamma2=gamma_er+gamma1;

    % �������� ���� ������� � ���������� ��
    g0=g*(1+bet*sin(fi(i-1))^2+bet1*sin(2*fi(i-1))^2);
    g_y=g0*sin(2*fi(i-1))*H*((e^2)/a-2*q)/a;
    g_z=g0+H*((3*H/a)-2*q*g*cos(fi(i-1))^2+e^2*(3*sin(fi(i-1))^2-1)-q*(1+6*sin(fi(i-1))^2))/a;
    
    % ��������� ��������� � ���������� ��
    n_e=d_V_e-(omega_H_1+2*u_H)*V_n(i-1)+(omega_n_1+2*u_n)*V_H;
    n_n=d_V_n+(omega_H_1+2*u_H)*V_e(i-1)-omega_e_1*V_H-g_y;
    n_H=-(omega_n_1+2*u_n)*V_e(i-1)+omega_e_1*V_n(i-1)-g_z;
    
    % ������� ������������ ���������
    C=[-sin(psi_list(i-1))*cos(tetta), sin(psi_list(i-1))*sin(tetta)*cos(gamma)+cos(psi_list(i-1))*sin(gamma), -sin(psi_list(i-1))*sin(tetta)*sin(gamma)+cos(psi_list(i-1))*cos(gamma);
        cos(psi_list(i-1))*cos(tetta), -cos(psi_list(i-1))*sin(tetta)*cos(gamma)+sin(psi_list(i-1))*sin(gamma), cos(psi_list(i-1))*sin(tetta)*cos(gamma)+sin(psi_list(i-1))*cos(gamma);
        sin(tetta), cos(tetta)*cos(gamma), -cos(tetta)*sin(gamma)];
    % �������� �� ���������� � ��������� ��
    omega_id=[omega_e;omega_n;omega_H];
    omega_c=C^-1*omega_id;
    omega_x=omega_c(1);
    omega_y=omega_c(2);
    omega_z=omega_c(3);
    
    n_id=[n_e;n_n;n_H];
    n_c=C^-1*n_id;
    n_x=n_c(1);
    n_y=n_c(2);
    n_z=n_c(3);
    % �������� �������� ��������� ���������� ������� �������� �������� �� ��������� �����
    dot_omega_H=(u*cos(fi(i-1))*V_n(i-1)+n_e*tan(fi(i-1))+V_H*omega_H+V_e(i-1)*V_n(i-1)/(ro2*cos(fi(i-1))^2))/ro2;
    
    %  ������� �������� ������� ������
    F=zeros(19);
    F(1,3)=1;
    F(2,4)=1;
    F(3,:)=[-w0^2+omega_n^2+omega_H^2,dot_omega_H-omega_e*omega_n,0,2*omega_H,0,n_H,0,0,0,0,0,0,0,C(1,1),C(1,2),C(1,3),n_x*C(1,1),n_y*C(1,2),n_z*C(1,3)];
    F(4,:)=[-(dot_omega_H-omega_e*omega_n),-w0^2+omega_e^2+omega_H^2,-2*omega_H,0,-n_H,0,n_e,0,0,0,0,0,0,C(2,1),C(2,2),C(2,3),n_x*C(2,1),n_y*C(2,2),n_z*C(2,3)];
    F(5,:)=[0,0,0,0,0,omega_H,-omega_n,C(1,1),C(1,2),C(1,3),omega_x*C(1,1),omega_y*C(1,2),omega_z*C(1,3),0,0,0,0,0,0];
    F(6,:)=[0,0,0,0,-omega_H,0,omega_e,C(2,1),C(2,2),C(2,3),omega_x*C(2,1),omega_y*C(2,2),omega_z*C(2,3),0,0,0,0,0,0];
    F(7,:)=[0,0,0,0,omega_H,-omega_e,0,C(3,1),C(3,2),C(3,3),omega_x*C(3,1),omega_y*C(3,2),omega_z*C(3,3),0,0,0,0,0,0];
    F_list(:,:,i-1)=F;
        
    % ������� ����� �������
    G=zeros(19,19);
    G(3,:)=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,C(1,1),C(1,2),C(1,3)];
    G(4,:)=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,C(2,1),C(2,2),C(2,3)];
    G(5,:)=[0,0,0,0,0,0,0,0,0,0,0,0,0,C(1,1),C(1,2),C(1,3),0,0,0];
    G(6,:)=[0,0,0,0,0,0,0,0,0,0,0,0,0,C(2,1),C(2,2),C(2,3),0,0,0];
    G(7,:)=[0,0,0,0,0,0,0,0,0,0,0,0,0,C(3,1),C(3,2),C(3,3),0,0,0];
    G_list(:,:,i-1)=G;
    % �������

    X_full=[x1; x2; x3; x4; alfa2; betta2; gamma2; gyro_zero_error; gyro_zero_error; gyro_zero_error; gyro_koef_error; gyro_koef_error; gyro_koef_error; ac_zero_error; ac_zero_error; ac_zero_error; ac_koef_error; ac_koef_error; ac_koef_error];

    W=[0,0,0,0,0,0,0,0,0,0,0,0,0,gyro_noise, gyro_noise, gyro_noise, ac_noise, ac_noise, ac_noise]';
    
    % �������
    
    X_out=(F*X_full+G*randn*W)*dt;
    X_out_list(:,i-1)=X_out;
    X_full_list(:,i-1)=X_full;
    

    %% ������
    
    Hk=zeros(4,19);% ������� ����� ������� ���� � ������� ���������
    Hk(1,1)=1/(ro2*cos(fi(i-1)));
    Hk(2,2)=1/ro1;
    Hk(3,:)=[-(V_H/ro2+omega_e*tan(fi(i-1))),-omega_H,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
    Hk(4,:)=[-V_H/ro1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
    H_list(:,:,i-1)=H;
    noise_lam=5/ro2*cos(fi(i-1));% ������� ������������ ���������� ������
    noise_fi=5/ro1;% ������� ������������ ��������� ������

    noise_V_e=.05;% ������� ������������ ����������� ��������� �������� ��������
    noise_V_n=.05;% ������� ������������ ����������� �������� �������� ��������
    VV=[noise_lam;noise_fi;noise_V_e;noise_V_n];% ������ ����� ��������� ���
    

    Zk=Hk*X_full+randn*VV;% ������ ��������� �������
    Zk_list(:,i-1)=Zk;
    
    
    %% ���%%%%%%%%%%%%%%���%%%
    % ���������� ���� ���������
    Rk=diag(VV);
    Rk=Rk.^2;
    Rk=Rk/dt;
    % ���������� ���� ��������
    Qk=diag(W);
    Qk=Qk.^2;
    Qk=Qk/dt;
    if i==2
        Pk_o=diag(X_full);
        Pk_o=Pk_o.^2;
        X_m=X_full;
    end
    
    Fk=(eye(19)+F*dt+((F*dt)^2)/2); % ������� �������� ������� � ���������� �����
    
    Gk=(eye(19)+F*dt/2+((F*dt)^2)/6)*G*dt; % ������� ����� ������� � ���������� ���� 
    
    
    Sk=Fk*Pk_o*Fk'+Gk*Qk*Gk';% ��������� ������� ���������� (������)
    
    K=Sk*Hk'*(Hk*Sk*Hk'+Rk)^-1;% ������� ������������� ��������
    
    Pk_o=(eye(19)-K*Hk)*Sk;% ������������� ������� ���������� (�������)
    
    X_m=Fk*X_m+K*(Zk-Hk*Fk*X_m);% ���������� ������
    
    X_k=X_full-X_m;% ������ ��������������� ������
    % ������������ �������� ��� ���������� �������� ���������������
    % �������� ������� � ���
    X_k_list(:,i-1)=X_k;
    X_m_list(:,i-1)=X_m;
    Pk_list(1,i-1)=sqrt(Pk_o(1,1));
    Pk_list(2,i-1)=sqrt(Pk_o(2,2));
    Pk_list(3,i-1)=sqrt(Pk_o(3,3));
    Pk_list(4,i-1)=sqrt(Pk_o(4,4));
    
    
    
end

%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%�������%%%%%%�������%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%



figure;
grid on
plot(X_k_list(2,:),X_k_list(1,:))
xlabel('x2 [�]')
ylabel('x1 [�]')

%%% �����������
figure
subplot(2,1,1)
grid on
hold on
plot(X_full_list(1,:),'r')
plot(X_k_list(1,:),'b','LineWidth',2)
plot(X_m_list(1,:),'g')
legend({'x1',
    'x1-������',
    '������'})
title("������ ����������� ���������� �� ���������� ������")
xlabel('����� ��������')
ylabel('x1 [�]')
subplot(2,1,2)
grid on
hold on
plot(X_full_list(2,:),'r')
plot(X_k_list(2,:),'b','LineWidth',2)
plot(X_m_list(2,:),'g')
legend({'x2',
    'x2-������',
    '������'})
title("������ ����������� ���������� �� ��������� ������")
xlabel('����� ��������')
ylabel('x2 [�]')

figure
subplot(2,1,1)
grid on
hold on
plot(X_full_list(3,:),'r')
plot(X_k_list(3,:),'b','LineWidth',2)
plot(X_m_list(3,:),'g')
legend({'x3',
    'x3-������',
    '������'})
title("������ ����������� ��������� ������������ ��������")
xlabel('����� ��������')
ylabel('x3 [�/c]')
subplot(2,1,2)
grid on
hold on
plot(X_full_list(4,:),'r')
plot(X_k_list(4,:),'b','LineWidth',2)
plot(X_m_list(4,:),'g')
legend({'x4',
    'x4-������',
    '������'})
title("������ ����������� �������� ������������ ��������")
xlabel('����� ��������')
ylabel('x4 [�/c]')

figure
grid on
hold on
plot(X_k_list(5,:),'r')
plot(X_k_list(6,:),'b')
plot(X_k_list(7,:),'g')
title("������ ����������")
legend({'alfa',
    'betta',
    'gamma'})
xlabel('����� ��������')
ylabel('[���]')
%% ��������������
figure;
subplot(4,1,1)
grid on
hold on
title("��� ������ ������ �� �������")
xlabel('����� ��������')
ylabel('[�]')
plot(Pk_list(1,:),'r')

subplot(4,1,2)
grid on
hold on
title("��� ������ ������ �� ������")
xlabel('����� ��������')
ylabel('[�]')
plot(Pk_list(2,:),'y')

subplot(4,1,3)
grid on
hold on
title("��� ������ ������ Ve")
xlabel('����� ��������')
ylabel('[�/c]')
plot(Pk_list(3,:),'b')

subplot(4,1,4)
grid on
hold on
title("��� ������ ������ Vn")
xlabel('����� ��������')
ylabel('[�/c]')
plot(Pk_list(4,:),'g')
figure;
plot(Zk_list(4,:))
