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
        return super().__init__(*args, **kwargs) 

    def read_min_data_series(self,id_turbine:int): 
        return self.connection.execute(AllQueryDB.query_to_min_data_series(id_turbine)).fetchall()

    def read_data_turbines(self,id_turbine:int): 
        return self.connection.execute(AllQueryDB.query_to_different_period(id_turbine)).fetchall() 
    
    def read_warning_and_error_turbine(self,id_turbine:int,date_init:str,date_finish:str):  
        return self.connection.execute(AllQueryDB.query_to_warning_and_error(id_turbine,date_init,date_finish)).fetchall() 

    def read_id_turbine(self):  
        return self.connection.execute(AllQueryDB.query_to_read_id_turbine()).fetchall() 