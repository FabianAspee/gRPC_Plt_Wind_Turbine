from AllQueryDB import AllQueryDB
import os

abspath = os.path.abspath(__file__)
dname = os.path.dirname(abspath)
os.chdir(dname)
import numpy as np
from sqlalchemy import create_engine
from sqlalchemy.engine import Engine
from sqlalchemy.exc import SQLAlchemyError


class ReadDB:
    """description of class"""

    @staticmethod
    def __connection_sqlite__():
        return create_engine('sqlite:///..\\PltWindTurbine\\Resources\\DatabaseTurbine\\plt_wind_turbine.db')

    def __init__(self, *args, **kwargs):
        connection = ReadDB.__connection_sqlite__()
        self.connection = connection.connect()
        super().__init__(*args, **kwargs)

    def read_min_data_series(self, id_turbine: int):
        return self.connection.execute(AllQueryDB.query_to_min_data_series(id_turbine)).fetchall()

    def read_data_turbines(self, id_turbine: int):
        return self.connection.execute(AllQueryDB.query_to_different_period(id_turbine)).fetchall()

    def read_name_turbine(self, id_turbine: int):
        return self.connection.execute(AllQueryDB.query_to_read_name_turbine(id_turbine)).fetchall()

    def read_data_error_turbines(self, id_turbine: int):
        return self.connection.execute(AllQueryDB.query_to_select_date_error(id_turbine)).fetchall()

    def read_warning_and_error_turbine(self, id_turbine: int, date_init: str, date_finish: str):
        return self.connection.execute(
            AllQueryDB.query_to_warning_and_error(id_turbine, date_init, date_finish)).fetchall()

    def read_id_turbine(self):
        return self.connection.execute(AllQueryDB.query_to_read_id_turbine()).fetchall()

    def read_all_maintenance_turbine(self):
        return self.connection.execute(AllQueryDB.query_to_all_maintenance_turbine()).fetchall()

    def read_nacelle_and_wind_direction(self, id_turbine, date_init, date_finish):
        return self.connection.execute(
            AllQueryDB.query_to_nacelle_and_wind_direction(id_turbine, date_init, date_finish)).fetchall()

    def read_angle_direction(self, id_turbine, date_init, date_finish):
        return self.connection.execute(
            AllQueryDB.query_to_nacelle_and_wind_direction(id_turbine, date_init, date_finish)).fetchall()

    def read_min_data_series_to_maintenance(self):
        all_info = []
        for (id_turbine,) in self.read_id_turbine():
            all_info.append((id_turbine, self.read_min_data_series(id_turbine)[0][4]))
        return all_info

    def read_max_data_series(self, id_turbine):
        return self.connection.execute(AllQueryDB.query_to_max_data_series(id_turbine)).fetchall()