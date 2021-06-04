class AllQueryDB:
    """description of class"""

    @staticmethod
    def query_to_different_period(id_turbine:int):
        return f'SELECT * FROM maintenance_turbine WHERE id_turbine={id_turbine} ORDER BY date'

    @staticmethod
    def query_to_warning_and_error(id_turbine:int,date_init:str,date_finish:str):
        return f"SELECT * FROM value_sensor_error WHERE id_turbine={id_turbine} AND date>='{date_init}'" \
               f"AND date<='{date_finish}' ORDER BY date "

    @staticmethod
    def query_to_min_data_series(id_turbine:int):
        return f'SELECT  * FROM value_sensor_turbine WHERE id_turbine={id_turbine} ORDER BY date LIMIT 1'

    @staticmethod
    def query_to_read_id_turbine():
        return f'SELECT id FROM wind_turbine_info'

    @staticmethod
    def query_to_all_maintenance_turbine():
        return f'SELECT * FROM maintenance_turbine'