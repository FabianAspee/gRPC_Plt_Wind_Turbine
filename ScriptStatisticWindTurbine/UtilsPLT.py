from CaseClassPlt import DateTurbine, TotalWarning
from datetime import datetime 
from functools import reduce 
from tail_recursion import tail_recursive, recurse 
  
errors = [180, 3370, 186, 182, 181]
warnings = [892, 891, 183, 79, 356 ] 
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

def sum_error_period(all_value_period:list):
    number = 0
    for value in all_value_period:  
        if value in errors: 
            number = number+1  
    return number

def verify_next_50(element:list,index:int,size_period:int,final:list,new_val:list,number_time:int,all_value_period_len:int):
    @tail_recursive
    def __verify_next_50__(element_aux:list,index_aux:int,final:list,new_val_aux:list=[],index_stop=0): 
        head, *tail = element_aux 
        if len(tail)==0: 
            return  element,index,size_period,final
        if head not in errors and index_stop<50:
             new_val_aux.append(head) 
             return recurse(tail,index_aux+1,final,new_val_aux,index_stop+1) 
        elif index_stop == 50:
            final.append(new_val)
            return element,index,size_period,final
        elif head in errors:
            new_val_aux.append(head)  
            final.append(new_val[len(new_val_aux):]+new_val_aux) 
            size_period_aux = int((all_value_period_len-index)/(1 if number_time==len(final) else number_time-len(final)))
            return tail,index_aux,size_period_aux,final
    return __verify_next_50__(element,index,final)

def insert_new_value_to_final(new_value:list,val:object,final:list):
    new_value.append(val) 
    if len(new_value)>49:
        final.append(new_value)
    return final

def verify_value_period(all_value_period:list,number_time:int,size_period:int):  
    @tail_recursive
    def __verify_value_period__(all_value_period_aux:list,index:int,size_period:int, count_aux:int=0,new_value:list=[],final:list=[]):
        head, *tail = all_value_period_aux 
        if len(tail)==0: 
            return insert_new_value_to_final(new_value,head,final)
        if head not in errors and count_aux <size_period and len(tail)>0:
            new_value.append(head)
            return recurse(tail,index+1,size_period,count_aux + 1,new_value,final) 
        elif count_aux == size_period and len(tail)>0: 
            tail,index,size_period,final=verify_next_50([head]+tail,index,size_period,final,new_value,number_time,len(all_value_period))
            return recurse(tail,index+1,size_period,1,[],final) 
        elif head in errors:
            final=insert_new_value_to_final(new_value,head,final)
            new_value = [] 
            size_period = int((len(all_value_period)-index)/(1 if number_time==len(final) else number_time-len(final)))
            return recurse(tail,index+1,size_period,0,new_value,final)
    return __verify_value_period__(all_value_period,1,size_period) 

def divide_serie_same_len(all_value_period:list,number_time:int)->list: 
    all_value = list(map(lambda val:val[3], all_value_period[1])) 
    number_error = sum_error_period(all_value)
    number_time =number_error if number_error>number_time else number_time 
    value_divide = int(len(all_value_period[1])/number_time)
    return verify_value_period(all_value,number_time,value_divide) 

def split_series_by_error(all_value_period:list):
    sub_series = []
    final_sub_series = []
    for val in all_value_period:
        if val not in errors:
            sub_series.append(val)
        if val in errors:
            sub_series.append(val)
            final_sub_series.append(sub_series)
            sub_series=[]
    return final_sub_series

def count_warning_by_sub_serie(single_sub_serie:list)->list:
    total = 0
    for val in single_sub_serie:
        if val in warnings:
            total = total+1
    return TotalWarning(total,val)

def search_warning_after_failure(all_value_period:list):
    all_value = list(map(lambda val:val[3], all_value_period[1])) 
    all_sub_series=split_series_by_error(all_value)
    return [count_warning_by_sub_serie(single_sub_serie) for single_sub_serie in all_sub_series]