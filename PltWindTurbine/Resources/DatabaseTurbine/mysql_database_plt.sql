CREATE TABLE IF NOT EXISTS   error_code  (
   id  INT PRIMARY KEY AUTO_INCREMENT,
   id_error_sensor  INT,
   ec_name  VARCHAR(255),
   wtg_event  INT,
   type  VARCHAR(255),
   description  VARCHAR(255),
   is_downtime  BOOLEAN,
   is_fault  BOOLEAN,
   cause  VARCHAR(255),
   nature  VARCHAR(255),
   reset_type  VARCHAR(255),
   alarm_level  INT,
   notes  VARCHAR(255),
   changed  VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS value_sensor_turbine  (
   id  INT PRIMARY KEY AUTO_INCREMENT,
   id_turbine  INT,
   id_sensor  INT,
   value  FLOAT,
       date  VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS  value_sensor_error  (
   id  INT PRIMARY KEY AUTO_INCREMENT,
   id_turbine  INT,
   id_error_sensor  INT,
   value  FLOAT,
   date  VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS  wind_turbine_info  (
        id  INT PRIMARY KEY AUTO_INCREMENT,
       access_point  VARCHAR(255),
       wind_power_plant  VARCHAR(255),
       folder_name  VARCHAR(255),
       tension_line  VARCHAR(255),
       turbine_name  VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS  sensor_info  (
       id  INT PRIMARY KEY AUTO_INCREMENT,
       sensor_name  VARCHAR(255),
       data_type_turbine  VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS  error_sensor  (
       id  INT PRIMARY KEY AUTO_INCREMENT,
       sensor_name  VARCHAR(255),
       data_type_turbine  VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS chart_system(
    id INT PRIMARY KEY AUTO_INCREMENT,
    chart_name  VARCHAR(255)
    );

ALTER TABLE  value_sensor_error  ADD FOREIGN KEY ( id_error_sensor ) REFERENCES  error_sensor  ( id );

ALTER TABLE  value_sensor_error  ADD FOREIGN KEY ( id_turbine ) REFERENCES  wind_turbine_info  ( id );

ALTER TABLE  error_code  ADD FOREIGN KEY ( id_error_sensor ) REFERENCES  error_sensor  ( id );

ALTER TABLE  value_sensor_turbine  ADD FOREIGN KEY ( id_turbine ) REFERENCES  wind_turbine_info  ( id );

ALTER TABLE  value_sensor_turbine  ADD FOREIGN KEY ( id_sensor ) REFERENCES  sensor_info  ( id );