﻿CREATE TABLE IF NOT EXISTS error_code(
    id INTEGER PRIMARY KEY,
    id_error_sensor INTEGER,
    ec_name  TEXT,
    wtg_event INTEGER,
    type TEXT,
    description TEXT,
    is_downtime NUMERIC,
    is_fault NUMERIC,
    cause TEXT,
    nature TEXT,
    reset_type TEXT,
    alarm_level int,
    notes TEXT,
    changed TEXT,
    FOREIGN KEY(id_error_sensor) REFERENCES error_sensor(id));

CREATE TABLE IF NOT EXISTS value_sensor_turbine(
    id INTEGER PRIMARY KEY,
    id_turbine INTEGER,
    id_sensor INTEGER,
    value REAL,
    date TEXT,
    FOREIGN KEY(id_turbine) REFERENCES wind_turbine_info(id),
    FOREIGN KEY(id_sensor) REFERENCES sensor_info(id));


CREATE TABLE IF NOT EXISTS wind_turbine_info (
    id INTEGER PRIMARY KEY,
    access_point TEXT,
    wind_power_plant TEXT,
    folder_name TEXT,
    tension_line TEXT,
    turbine_name TEXT);

CREATE TABLE IF NOT EXISTS sensor_info(
    id INTEGER PRIMARY KEY,
    sensor_name TEXT,
    sensor_data_type TEXT);

CREATE TABLE IF NOT EXISTS value_sensor_error (
   id INTEGER PRIMARY KEY ,
   id_turbine INTEGER,
   id_error_sensor INTEGER,
   value REAL,
   date TEXT,
   FOREIGN KEY(id_turbine) REFERENCES wind_turbine_info(id),
   FOREIGN KEY(id_error_sensor) REFERENCES error_sensor(id)
);

CREATE TABLE IF NOT EXISTS error_sensor(
    id INTEGER PRIMARY KEY,
    sensor_name TEXT,
    data_type_turbine TEXT);

CREATE TABLE IF NOT EXISTS chart_system(
    id INTEGER PRIMARY KEY,
    chart_name TEXT);
   
CREATE TABLE IF NOT EXISTS model_system(
    id INTEGER PRIMARY KEY,
    model_name TEXT
    );

CREATE TABLE IF NOT EXISTS own_serie_turbine(
    id INTEGER PRIMARY KEY,
    name TEXT,
    is_ok NUMERIC,
    sensor_data_type TEXT);

CREATE TABLE IF NOT EXISTS value_own_serie_turbine(
    id INTEGER PRIMARY KEY,
    id_turbine INTEGER,
    id_own_serie INTEGER,
    value REAL,
    date TEXT,
    FOREIGN KEY(id_turbine) REFERENCES wind_turbine_info(id),
    FOREIGN KEY(id_own_serie) REFERENCES own_serie_turbine(id)
    ); 

    
CREATE TABLE IF NOT EXISTS maintenance_turbine(
    id INTEGER PRIMARY KEY,
    id_turbine INTEGER,
    date TEXT,
    FOREIGN KEY(id_turbine) REFERENCES wind_turbine_info(id));

INSERT INTO chart_system(chart_name) 
SELECT 'Line Chart' 
WHERE NOT EXISTS(SELECT 1 FROM chart_system WHERE chart_name = 'Line Chart');

INSERT INTO chart_system(chart_name) 
SELECT 'Line Chart with Warning' 
WHERE NOT EXISTS(SELECT 1 FROM chart_system WHERE chart_name = 'Line Chart with Warning');

INSERT INTO chart_system(chart_name) 
SELECT 'Scatter Chart' 
WHERE NOT EXISTS(SELECT 1 FROM chart_system WHERE chart_name = 'Scatter Chart');

INSERT INTO chart_system(chart_name) 
SELECT 'Scatter Chart with Warning' 
WHERE NOT EXISTS(SELECT 1 FROM chart_system WHERE chart_name = 'Scatter Chart with Warning');

INSERT INTO chart_system(chart_name) 
SELECT 'Radar Chart' 
WHERE NOT EXISTS(SELECT 1 FROM chart_system WHERE chart_name = 'Radar Chart');

INSERT INTO chart_system(chart_name) 
SELECT 'Line Chart Draw Warning' 
WHERE NOT EXISTS(SELECT 1 FROM chart_system WHERE chart_name = 'Line Chart Draw Warning');

INSERT INTO chart_system(chart_name) 
SELECT 'Bar Chart Draw Warning' 
WHERE NOT EXISTS(SELECT 1 FROM chart_system WHERE chart_name = 'Bar Chart Draw Warning'); 

INSERT INTO own_serie_turbine(name,is_ok,sensor_data_type) 
SELECT 'Angle',0,'DMAV' 
WHERE NOT EXISTS(SELECT 1 FROM own_serie_turbine WHERE name = 'Angle');  

 