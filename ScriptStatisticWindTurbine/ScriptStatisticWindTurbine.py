from ReadDB import ReadDB as db
from dataclasses import dataclass
from datetime import datetime 
import pandas as pd
import numpy as np

@dataclass(frozen=True)
class DateTurbine:
    date_init: str
    date_finish: str


def create_final_list_with_date(all_dates_by_turbine:list)->list:
    def _create_final_list_with_date_(all_dates_by_turbine:list,custom_list:list)->list:
        if(len(all_dates_by_turbine)>=2):
            custom_list.append(DateTurbine(all_dates_by_turbine[0].date_finish,all_dates_by_turbine[1].date_init))
            return _create_final_list_with_date_(all_dates_by_turbine[1:],custom_list)
        elif(len(all_dates_by_turbine)==1):
            custom_list.append(DateTurbine(all_dates_by_turbine[0].date_finish,datetime.now().strftime("%Y/%m/%d, %H:%M:%S")))
            return custom_list
        else:
            return custom_list

    custom_list:list = []
    if(len(all_dates_by_turbine)>=2):
        custom_list.append(DateTurbine(all_dates_by_turbine[0].date_finish,all_dates_by_turbine[1].date_init))
        return _create_final_list_with_date_(all_dates_by_turbine[1:],custom_list)
    else:
        return None

if __name__ == '__main__':
    db_call = db() 
    for (id_turbine,) in db_call.read_id_turbine():
        all_dates_by_turbine = list(map(lambda value:DateTurbine("",value[4]),db_call.read_min_data_series(id_turbine)))+list(map(lambda value:DateTurbine(value[2],value[3]),db_call.read_data_turbines(id_turbine)))
        date_to_query = create_final_list_with_date(all_dates_by_turbine)
        if(date_to_query is not None):
            save_all_series_result:list = []
            for value in date_to_query: 
                result = db_call.read_warning_and_error_turbine(1,value.date_init,value.date_finish)
                save_all_series_result.append((len(result),result))
        
            save_all_series_result = list(filter(lambda filter_value: filter_value[0]!=0 ,save_all_series_result))
            final_len = min(list(map(lambda value_turbine:value_turbine[0],save_all_series_result)), key=lambda x:x)
            final_element = list(map(lambda value: list(map(lambda values:values[3],value[1][value[0]-final_len:])),save_all_series_result)) 
            new_array =np.array(final_element).reshape(final_len,-1)  
            print(new_array.shape)
            df = pd.DataFrame(new_array)
            df = df.replace('null','NaN')
            for col in df.columns:
                df = df.astype({col: float}) 
         
            print(df.corr())