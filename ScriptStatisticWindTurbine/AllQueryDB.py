class AllQueryDB:
    """description of class"""
    __nacelle_sensor__ = 2
    __wind_dir_sensor__ = 4
    __active_sensor__ = 1
    __angle_sensor__ = 1
    __errors__ = [180, 3370, 186, 182, 181]

    @staticmethod
    def query_to_different_period(id_turbine: int):
        return f'SELECT * FROM maintenance_turbine WHERE id_turbine={id_turbine} ORDER BY date'

    @staticmethod
    def query_to_select_date_error(id_turbine: int):
        return f'SELECT * FROM value_sensor_error WHERE id_turbine={id_turbine} AND value IN ' \
               f'{tuple(AllQueryDB.__errors__)} ORDER BY date'

    @staticmethod
    def query_to_warning_and_error(id_turbine: int, date_init: str, date_finish: str):
        return f"SELECT * FROM value_sensor_error WHERE id_turbine={id_turbine} AND date>='{date_init}'" \
               f"AND date<='{date_finish}' ORDER BY date "

    @staticmethod
    def query_to_nacelle_and_wind_direction(id_turbine: int, date_init: str, date_finish: str):
        return f"SELECT * FROM value_sensor_turbine WHERE id_turbine={id_turbine} AND id_sensor IN " \
               f"{AllQueryDB.__nacelle_sensor__, AllQueryDB.__wind_dir_sensor__, AllQueryDB.__active_sensor__} " \
               f"AND date>='{date_init}' AND date<='{date_finish}' ORDER BY date "

    @staticmethod
    def query_to_min_data_series(id_turbine: int):
        return f'SELECT  * FROM value_sensor_turbine WHERE id_turbine={id_turbine} ORDER BY date LIMIT 1'

    @staticmethod
    def query_to_read_id_turbine():
        return f'SELECT id FROM wind_turbine_info'

    @staticmethod
    def query_to_read_name_turbine(id_turbine: int):
        return f'SELECT turbine_name FROM wind_turbine_info where id = {id_turbine}'

    @staticmethod
    def query_to_all_maintenance_turbine():
        return f'SELECT * FROM maintenance_turbine'
