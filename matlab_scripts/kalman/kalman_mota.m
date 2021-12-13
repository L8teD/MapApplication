clc; clear; close all;

dataTable = readtable('matlabData.csv', 'Format', '%f%f%f%f%f%f%f%f%f%f%f%f%f%f%f%f%f%f%f%f%f%f%f%f');
dataTable.Properties.VariableNames = {'lat', 'lon', 'heading', 'roll', 've', 'vn', 'r1', 'r2', 'aw_e', 'aw_n', 'aw_h', 'alfa' , 'betta', 'gamma','accE','accN','accH','w_x','w_y','w_z','n_x','n_y','n_z','omega_Z_dot'};

lam = dataTable.lon;
fi = dataTable.lat;
psi_list = dataTable.heading;
gam_list = dataTable.roll;
V_e = dataTable.ve;
V_n = dataTable.vn;
ro1 = dataTable.r1;
ro2 = dataTable.r2;
aw_e = dataTable.aw_e;
aw_n = dataTable.aw_n;
aw_h = dataTable.aw_h;
alfa = dataTable.alfa;
betta = dataTable.betta;
gamma = dataTable.gamma;
acc_E = dataTable.accE;
acc_N = dataTable.accN;
acc_H = dataTable.accH;
w_x = dataTable.w_x;
w_y = dataTable.w_y;
w_z = dataTable.w_z;
n_x = dataTable.n_x;
n_y = dataTable.n_y;
n_z = dataTable.n_z;
for i = 1:length(psi_list)
    psi_list(i)=-psi_list(i)+2*pi;
end
omega_Z_dot = dataTable.omega_Z_dot;
[X_k_list, X_full_list, X_m_list, Pk_list, Zk_list] = Kalman(lam,fi, psi_list, gam_list ,V_e, V_n, ro1, ro2,aw_e, aw_n, aw_h, alfa, betta, gamma, acc_E, acc_N, acc_H, w_x, w_y, w_z,n_x, n_y,n_z,omega_Z_dot);

figure;
grid on
plot(X_k_list(2,:),X_k_list(1,:))
xlabel('x2 [м]')
ylabel('x1 [м]')

figure
subplot(2,1,1)
grid on
hold on
plot(X_full_list(1,:),'r')
plot(X_k_list(1,:),'b','LineWidth',2)
plot(X_m_list(1,:),'g')
legend({'x1',
    'x1-оценка',
    'оценка'})
title("Ошибка определения координаты по долготному каналу")
xlabel('число итераций')
ylabel('x1 [м]')
subplot(2,1,2)
grid on
hold on
plot(X_full_list(2,:),'r')
plot(X_k_list(2,:),'b','LineWidth',2)
plot(X_m_list(2,:),'g')
legend({'x2',
    'x2-оценка',
    'оценка'})
title("Ошибка определения координаты по широтному каналу")
xlabel('число итераций')
ylabel('x2 [м]')

figure
subplot(2,1,1)
grid on
hold on
plot(X_full_list(3,:),'r')
plot(X_k_list(3,:),'b','LineWidth',2)
plot(X_m_list(3,:),'g')
legend({'x3',
    'x3-оценка',
    'оценка'})
title("Ошибка определения восточной составляющей скорости")
xlabel('число итераций')
ylabel('x3 [м/c]')
subplot(2,1,2)
grid on
hold on
plot(X_full_list(4,:),'r')
plot(X_k_list(4,:),'b','LineWidth',2)
plot(X_m_list(4,:),'g')
legend({'x4',
    'x4-оценка',
    'оценка'})
title("Ошибка определения северной составляющей скорости")
xlabel('число итераций')
ylabel('x4 [м/c]')

figure
grid on
hold on
plot(X_k_list(5,:),'r')
plot(X_k_list(6,:),'b')
plot(X_k_list(7,:),'g')
title("Ошибка ориентации")
legend({'alfa',
    'betta',
    'gamma'})
xlabel('число итераций')
ylabel('[рад]')

figure;
subplot(4,1,1)
grid on
hold on
title("СКО ошибки оценки по долготе")
xlabel('число итераций')
ylabel('[м]')
plot(Pk_list(1,:),'r')

subplot(4,1,2)
grid on
hold on
title("СКО ошибки оценки по широте")
xlabel('число итераций')
ylabel('[м]')
plot(Pk_list(2,:),'y')

subplot(4,1,3)
grid on
hold on
title("СКО ошибки оценки Ve")
xlabel('число итераций')
ylabel('[м/c]')
plot(Pk_list(3,:),'b')

subplot(4,1,4)
grid on
hold on
title("СКО ошибки оценки Vn")
xlabel('число итераций')
ylabel('[м/c]')
plot(Pk_list(4,:),'g')
figure;
plot(Zk_list(4,:))
