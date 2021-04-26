CREATE TABLE IF NOT EXISTS error_code(
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
    chart_name TEXT
    );