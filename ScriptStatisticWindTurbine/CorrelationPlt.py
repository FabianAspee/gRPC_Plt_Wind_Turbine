from ReadDB import ReadDB as db
from datetime import datetime
import pandas as pd
import numpy as np
from CaseClassPlt import DateTurbine
import UtilsPLT as utilplt

db_call = db()


def get_all_date_by_turbine(id_turbine: int) -> list:
    """
    this method return a list with all date of maintenance for a turbine
    :param id_turbine: id of a turbine
    :return: list with date turbine in maintenance period
    """
    return list(map(lambda value: DateTurbine("", value[4], False), db_call.read_min_data_series(id_turbine))) + list(
        map(lambda value: DateTurbine(value[2], value[3], bool(value[4])), db_call.read_data_turbines(id_turbine)))


def calculate_correlation_between_period():
    """
    this method calculus correlation inside a maintenance period, if is a normal maintenance period or extra, this is
    calculate in this method
    :return:
    """
    for value_series, id_turbine in get_all_series_maintenance_by_turbine():
        final_len = min(list(map(lambda value_turbine: value_turbine[0], value_series)), key=lambda x: x)
        final_element = list(map(lambda value: list(map(lambda values: values[3], value[1][value[0] - final_len:])),
                                 value_series))
        calculus_correlation(final_len, final_element, id_turbine)


def calculate_correlation_divide_period(number_time: int):
    """
    this method calculus correlation between periods of maintenance, but, select initially all period (normal and
    extra), after this operation, try to divide the series in the number_time if is possible, but whilst try to
    divide the series, if found the more errors that number_time then the total quantity of series will be equal to
    quantity of errors :param number_time: number of time that a series is divide for calculus correlation :return:
    """
    values = ((val, id_turbine) for value_series, id_turbine in get_all_series_maintenance_by_turbine()
              for val in value_series)
    for value_series, id_turbine in values:
        final_save_all_series_result = utilplt.divide_series_same_len(value_series, number_time)
        if not not final_save_all_series_result:
            final_len = min(list(map(lambda value_turbine: len(value_turbine), final_save_all_series_result)),
                            key=lambda x: x)
            final_element = list(map(lambda value: value[len(value)-final_len:], final_save_all_series_result))
            calculus_correlation(final_len, final_element, id_turbine)


"""finire questo ancora"""

"""calculate corr by number time divide period and calculate corr sum all warning in the week"""
def calculate_correlation_sum_warning_week_period(number_time: int):
    for value_series, id_turbine in get_all_series_maintenance_by_turbine():
        final_series = [utilplt.divide_series_by_week(val) for val in value_series]
        final_len = min([len(list_value) for list_value in final_series], key=lambda x: x)
        final_element = list(map(lambda value: value[len(value)-final_len:], final_series))
        calculus_correlation(final_len, final_element, id_turbine)


def calculate_correlation_sum_warning_week_periods():
    """
    This method read all series of maintenance by turbine and sum all warning inside period to before calculate
    correlation between their.
    :return:
    """
    for value_series, id_turbine in get_all_series_maintenance_by_turbine():
        final_series = [utilplt.divide_series_by_week(val) for val in value_series]
        final_len = min([len(list_value) for list_value in final_series], key=lambda x: x)
        final_element = list(map(lambda value: value[len(value)-final_len:], final_series))
        calculus_correlation(final_len, final_element, id_turbine)


"""
[DateTurbine(date_init='2018/06/22 00:10:00', date_finish='2018/11/29 07:30:00', is_normal=False), 
DateTurbine(date_init='2018/11/29 08:00:00', date_finish='2018/12/20 11:00:00', is_normal=False), 
DateTurbine(date_init='2018/12/20 13:30:00', date_finish='2020/05/06 09:15:00', is_normal=True), 
DateTurbine(date_init='2020/05/06 15:45:00', date_finish='2021/06/04, 11:38:38', is_normal=True)]

[DateTurbine(date_init='', date_finish='2018/06/22 00:10:00', is_normal=False), 
DateTurbine(date_init='2018/11/29 07:30:00', date_finish='2018/11/29 08:00:00', is_normal=False), 
DateTurbine(date_init='2018/12/20 11:00:00', date_finish='2018/12/20 13:30:00', is_normal=True), 
DateTurbine(date_init='2020/05/06 09:15:00', date_finish='2020/05/06 15:45:00', is_normal=True)]
"""


def get_all_series_maintenance_by_turbine():
    """
    method that return all series by maintenance period for specific turbine
    :return:
    """
    for (id_turbine,) in db_call.read_id_turbine():
        all_dates_by_turbine = get_all_date_by_turbine(id_turbine)
        date_to_query = utilplt.create_final_list_with_date(all_dates_by_turbine)
        if date_to_query is not None:
            save_all_series_result: list = []
            for value in date_to_query:
                result = db_call.read_warning_and_error_turbine(id_turbine, value.date_init, value.date_finish)
                save_all_series_result.append((len(result), result))

            yield list(filter(lambda filter_value: filter_value[0] != 0, save_all_series_result)), id_turbine


def get_all_series_maintenance_inside_extra_maintenance_by_turbine():
    """
    method that return all series by maintenance period for specific turbine
    :return:
    """
    for (id_turbine,) in db_call.read_id_turbine():
        all_dates_by_turbine = get_all_date_by_turbine(id_turbine)
        date_to_query = utilplt.create_final_list_with_date(all_dates_by_turbine)
        if date_to_query is not None:
            save_all_series_result: list = []
            iterator_date = (val for val in date_to_query if val.is_normal is False)
            for value in iterator_date:
                result = db_call.read_warning_and_error_turbine(id_turbine, value.date_init, value.date_finish)
                save_all_series_result.append((len(result), result))

            yield list(filter(lambda filter_value: filter_value[0] != 0, save_all_series_result)), id_turbine


def reshape_array(final_len: int, element_to_reshape: list):
    return np.array(element_to_reshape).reshape(final_len, -1)


def calculate_correlation_inside_extra_maintenance():
    for value_series, id_turbine in get_all_series_maintenance_inside_extra_maintenance_by_turbine():
        if len(value_series) > 1:
            final_len = min(list(map(lambda val: val[0], value_series)), key=lambda x: x)
            final_element = list(map(lambda value: list(map(lambda val: val[3], value[1][len(value)-final_len:])),
                                     value_series))
            calculus_correlation(final_len, final_element, id_turbine)


def calculus_correlation(final_len: int, final_element: list, id_turbine: int) -> None:
    new_array = reshape_array(final_len, final_element)
    print(f'SHAPE TURBINE {id_turbine} {new_array.shape}')
    df = pd.DataFrame(new_array)
    df = df.replace('null', 'NaN')
    for col in df.columns:
        df = df.astype({col: float})
    print(f'ID TURBINE {id_turbine} \n')
    print(f'{df.corr()} \n')


def search_warning_before_failure_inside_maintenance_period():
    for value_series, id_turbine in get_all_series_maintenance_by_turbine():
        for value_series_single in value_series:
            final_info = utilplt.search_warning_after_failure(value_series_single)
            print(f'ID TURBINE {id_turbine} \n')
            for info_by_error in final_info:
                print(f'Error Code {info_by_error.error_code} \n')
                print(f'Total Warning {info_by_error.total_warning} \n')
