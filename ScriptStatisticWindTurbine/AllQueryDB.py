from typing import List


class AllQueryDB:
    """description of class"""
    __nacelle_sensor__ = 2
    __wind_dir_sensor__ = 4
    __active_sensor__ = 1
    __rotor_sensor__ = 3
    __wind_speed_sensor__ = 5
    __angle_sensor__ = 1
    __errors__: List[int] = [180, 3370, 186, 182, 181]
    __warnings__: List[int] = [892, 891, 183, 79, 356]

    @staticmethod
    def query_to_different_period(id_turbine: int):
        return f'SELECT * FROM maintenance_turbine WHERE id_turbine={id_turbine} ORDER BY date'

    @staticmethod
    def query_to_select_normal_maintenance(id_turbine: int):
        return f'SELECT * FROM maintenance_turbine WHERE id_turbine={id_turbine} AND is_normal_maintenance' \
               f' ORDER BY date'

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
        return f'SELECT id FROM wind_turbine_info WHERE have_torquer'

    @staticmethod
    def query_to_read_name_turbine(id_turbine: int):
        return f'SELECT turbine_name FROM wind_turbine_info where id = {id_turbine}'

    @staticmethod
    def query_to_all_maintenance_turbine():
        return f'SELECT * FROM maintenance_turbine'

    @staticmethod
    def query_to_max_data_series(id_turbine):
        return f'SELECT  * FROM value_sensor_turbine WHERE id_turbine={id_turbine} ORDER BY date DESC LIMIT 1'

    @staticmethod
    def query_to_select_date_event(id_turbine):
        return f'SELECT * FROM value_sensor_error WHERE id_turbine={id_turbine} AND value IN ' \
               f'{tuple(AllQueryDB.__errors__+AllQueryDB.__warnings__)} ORDER BY date'

    @staticmethod
    def query_to_all_sensor_period(id_turbine, date_init, date_finish):
        return f"SELECT * FROM value_sensor_turbine WHERE id_turbine={id_turbine} AND id_sensor IN " \
               f"{AllQueryDB.__active_sensor__, AllQueryDB.__nacelle_sensor__, AllQueryDB.__rotor_sensor__, AllQueryDB.__wind_dir_sensor__, AllQueryDB.__wind_speed_sensor__ } " \
               f"AND date>='{date_init}' AND date<='{date_finish}' ORDER BY date "
