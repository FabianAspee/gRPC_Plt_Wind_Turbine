from ReadDB import ReadDB as db 
from datetime import datetime 
import pandas as pd
import numpy as np
from CaseClassPlt import DateTurbine
import UtilsPLT as utilplt 

db_call = db() 
def calculate_correlation_between_period():
    for (id_turbine,) in db_call.read_id_turbine():
        all_dates_by_turbine = list(map(lambda value:DateTurbine("",value[4]),db_call.read_min_data_series(id_turbine)))+list(map(lambda value:DateTurbine(value[2],value[3]),db_call.read_data_turbines(id_turbine)))
        date_to_query = utilplt.create_final_list_with_date(all_dates_by_turbine)
        if(date_to_query is not None):
            save_all_series_result:list = []
            for value in date_to_query: 
                result = db_call.read_warning_and_error_turbine(id_turbine,value.date_init,value.date_finish)
                save_all_series_result.append((len(result),result))
        
            save_all_series_result = list(filter(lambda filter_value: filter_value[0]!=0 ,save_all_series_result))
            final_len = min(list(map(lambda value_turbine:value_turbine[0],save_all_series_result)), key=lambda x:x)
            final_element = list(map(lambda value: list(map(lambda values:values[3],value[1][value[0]-final_len:])),save_all_series_result)) 
            new_array =np.array(final_element).reshape(final_len,-1)  
            print(f'SHAPE TURBINE {id_turbine} {new_array.shape}')
            df = pd.DataFrame(new_array)
            df = df.replace('null','NaN')
            for col in df.columns:
                df = df.astype({col: float}) 
            print(f'ID TURBINE {id_turbine} \n')
            print(f'{df.corr()} \n')

"""calculate corr by number time divide period and calculate corr sum all warning in the week"""
def calculate_correlation_divide_period(number_time:int):
    for (id_turbine,) in db_call.read_id_turbine():
        all_dates_by_turbine = list(map(lambda value:DateTurbine("",value[4]),db_call.read_min_data_series(id_turbine)))+list(map(lambda value:DateTurbine(value[2],value[3]),db_call.read_data_turbines(id_turbine)))
        date_to_query = utilplt.create_final_list_with_date(all_dates_by_turbine)
        if(date_to_query is not None):
            save_all_series_result:list = []
            for value in date_to_query: 
                result = db_call.read_warning_and_error_turbine(id_turbine,value.date_init,value.date_finish)
                save_all_series_result.append((len(result),result))
        
            save_all_series_result = list(filter(lambda filter_value: filter_value[0]!=0 ,save_all_series_result))
            for value_serie in save_all_series_result:  
                final_save_all_series_result = utilplt.divide_serie_same_len(value_serie,number_time)
                if not not final_save_all_series_result:
                    final_len = min(list(map(lambda value_turbine:len(value_turbine),final_save_all_series_result)), key=lambda x:x)
                    final_element = list(map(lambda value:value[:final_len] ,final_save_all_series_result)) 
                    new_array =np.array(final_element).reshape(final_len,-1)  
                    print(f'SHAPE TURBINE {id_turbine} {new_array.shape}')
                    df = pd.DataFrame(new_array)
                    df = df.replace('null','NaN')
                    for col in df.columns:
                        df = df.astype({col: float}) 
                    print(f'ID TURBINE {id_turbine} \n')
                    print(f'{df.corr()} \n')

def calculate_correlation_sum_warning_week_period(number_time:int):
    for (id_turbine,) in db_call.read_id_turbine():
        all_dates_by_turbine = list(map(lambda value:DateTurbine("",value[4]),db_call.read_min_data_series(id_turbine)))+list(map(lambda value:DateTurbine(value[2],value[3]),db_call.read_data_turbines(id_turbine)))
        date_to_query = utilplt.create_final_list_with_date(all_dates_by_turbine)
        if(date_to_query is not None):
            save_all_series_result:list = []
            for value in date_to_query: 
                result = db_call.read_warning_and_error_turbine(id_turbine,value.date_init,value.date_finish)
                save_all_series_result.append((len(result),result))
        
            save_all_series_result = list(filter(lambda filter_value: filter_value[0]!=0 ,save_all_series_result))
            for value_serie in save_all_series_result:  
                final_save_all_series_result = utilplt.divide_serie_same_len(value_serie,number_time)
                if not not final_save_all_series_result:
                    final_len = min(list(map(lambda value_turbine:len(value_turbine),final_save_all_series_result)), key=lambda x:x)
                    final_element = list(map(lambda value:value[:final_len] ,final_save_all_series_result)) 
                    new_array =np.array(final_element).reshape(final_len,-1)  
                    print(f'SHAPE TURBINE {id_turbine} {new_array.shape}')
                    df = pd.DataFrame(new_array)
                    df = df.replace('null','NaN')
                    for col in df.columns:
                        df = df.astype({col: float}) 
                    print(f'ID TURBINE {id_turbine} \n')
                    print(f'{df.corr()} \n')



def search_warning_after_failure_inside_maintenance_period():
    for (id_turbine,) in db_call.read_id_turbine():
        all_dates_by_turbine = list(map(lambda value:DateTurbine("",value[4]),db_call.read_min_data_series(id_turbine)))+list(map(lambda value:DateTurbine(value[2],value[3]),db_call.read_data_turbines(id_turbine)))
        date_to_query = utilplt.create_final_list_with_date(all_dates_by_turbine)
        if(date_to_query is not None):
            save_all_series_result:list = []
            for value in date_to_query: 
                result = db_call.read_warning_and_error_turbine(id_turbine,value.date_init,value.date_finish)
                save_all_series_result.append((len(result),result))
        
            save_all_series_result = list(filter(lambda filter_value: filter_value[0]!=0 ,save_all_series_result))
            for value_serie in save_all_series_result:  
                final_info = utilplt.search_warning_after_failure(value_serie) 
                print(f'ID TURBINE {id_turbine} \n')
                for info_by_error in final_info:
                    print(f'Error Code {info_by_error.error_code} \n')
                    print(f'Total Warning {info_by_error.total_warning} \n')