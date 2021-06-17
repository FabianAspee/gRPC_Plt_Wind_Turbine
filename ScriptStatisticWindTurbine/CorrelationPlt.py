from ReadDB import ReadDB as Db
from datetime import datetime
import pandas as pd
import numpy as np
from CaseClassPlt import DateTurbine, DateTurbineCustom, DateTurbineError
import UtilsPLT as Util_Plt

db_call = Db()


def get_min_and_max_date(id_turbine: int) -> DateTurbine:
    """
    return the min and max date in series turbine
    :param id_turbine: represent a id of the turbine in the system
    :return:
    """
    return list(map(lambda value: DateTurbine(value[0][4], value[1][4], False),
                    zip(db_call.read_min_data_series(id_turbine),db_call.read_max_data_series(id_turbine))))[0]


def get_all_date_by_turbine(id_turbine: int) -> list:
    """
    this method return a list with all date of maintenance for a turbine
    :param id_turbine: id of a turbine
    :return: list with date turbine in maintenance period
    """
    return list(map(lambda value: DateTurbine("", value[4], False),
                    db_call.read_min_data_series(id_turbine))) + get_all_init_date_by_turbine(id_turbine)


def get_all_date_by_turbine_finish_true(id_turbine: int) -> list:
    """
    this method return a list with all date of maintenance for a turbine
    the first date is w.r.t first value of sensor and
    ended date is w.r.t to final date of sensor both contains true for is_normal that indicate
    that a maintenance is a normal maintenance and not extra maintenance
    :param id_turbine: id of a turbine
    :return: list with date turbine in maintenance period
    """
    return list(map(lambda value: DateTurbine("", value[4], True),
                    db_call.read_min_data_series(id_turbine))) + get_all_init_date_by_turbine(id_turbine) + \
           list(map(lambda value: DateTurbine(value[4], value[4], True),
                    db_call.read_max_data_series(id_turbine)))


def get_all_init_date_by_turbine(id_turbine: int) -> list:
    """
    this method return a list with all date of maintenance for a turbine
    :rtype: object
    :param id_turbine: id of a turbine
    :return: list with date turbine in maintenance period
    """
    return list(
        map(lambda value: DateTurbine(value[2], value[3], bool(value[4])), db_call.read_data_turbines(id_turbine)))


def yield_get_all_date_error_by_turbine():
    """
    Select all date where an error happened
    :return:list with date of the turbine when error occurred
    """
    for (id_turbine,) in db_call.read_id_turbine():
        yield id_turbine, list(
            map(lambda value: DateTurbineError(value[4], value[3]), db_call.read_data_error_turbines(id_turbine)))


def calculate_correlation_between_period():
    """
    this method calculus correlation inside a maintenance period, if is a normal maintenance period or extra, this is
    calculate in this method
    :return:
    """
    for value_series, id_turbine in get_all_series_maintenance_by_turbine():
        final_len = search_min_value(value_series)
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
        final_save_all_series_result = Util_Plt.divide_series_same_len(value_series, number_time)
        if not not final_save_all_series_result:
            final_len = min(list(map(lambda value_turbine: len(value_turbine), final_save_all_series_result)),
                            key=lambda x: x)
            final_element = list(map(lambda value: value[len(value) - final_len:], final_save_all_series_result))
            calculus_correlation(final_len, final_element, id_turbine)


"""finire questo ancora"""

"""calculate corr by number time divide period and calculate corr sum all warning in the week"""


def calculate_correlation_sum_warning_week_period(number_time: int):
    for value_series, id_turbine in get_all_series_maintenance_by_turbine():
        final_series = [Util_Plt.divide_series_by_week(val) for val in value_series]
        final_len = min([len(list_value) for list_value in final_series], key=lambda x: x)
        final_element = list(map(lambda value: value[len(value) - final_len:], final_series))
        calculus_correlation(final_len, final_element, id_turbine)


def calculate_correlation_sum_warning_week_periods():
    """
    This method read all series of maintenance by turbine and sum all warning inside period to before calculate
    correlation between their.
    :return:
    """
    for value_series, id_turbine in get_all_series_maintenance_by_turbine():
        final_series = [Util_Plt.divide_series_by_week(val) for val in value_series]
        final_len = min([len(list_value) for list_value in final_series], key=lambda x: x)
        final_element = list(map(lambda value: value[len(value) - final_len:], final_series))
        calculus_correlation(final_len, final_element, id_turbine)


def yield_get_all_date_by_turbine():
    """
    Selects all maintenance period dates by turbine, adds the initial period, i.e.,
    the initial date of when the information for the series begins to exist
    :return:
    """
    for (id_turbine,) in db_call.read_id_turbine():
        yield id_turbine, get_all_date_by_turbine(id_turbine)


def yield_get_all_date_by_turbine_with_final():
    """
    Selects all maintenance period dates by turbine, adds the initial period, i.e.,
    the initial date of when the information for the series begins to exist and the final period when information
    existed
    :return:
    """
    for (id_turbine,) in db_call.read_id_turbine():
        yield id_turbine, get_all_date_by_turbine_finish_true(id_turbine)


def yield_get_all_date_by_turbine_original():
    """
    Select all date maintenance period by turbine
    :return:
    """
    for (id_turbine,) in db_call.read_id_turbine():
        yield id_turbine, get_all_init_date_by_turbine(id_turbine)


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
    for (id_turbine, all_dates_by_turbine) in yield_get_all_date_by_turbine():
        date_to_query = Util_Plt.create_final_list_with_date(all_dates_by_turbine)
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
        date_to_query = Util_Plt.create_final_list_with_date(all_dates_by_turbine)
        if date_to_query is not None:
            save_all_series_result: list = []
            iterator_date = (val for val in date_to_query if val.is_normal is False)
            for value in iterator_date:
                result = db_call.read_warning_and_error_turbine(id_turbine, value.date_init, value.date_finish)
                save_all_series_result.append((len(result), result))

            yield list(filter(lambda filter_value: filter_value[0] != 0, save_all_series_result)), id_turbine


def reshape_array(final_len: int, element_to_reshape: list):
    return np.array(element_to_reshape).reshape(final_len, -1)


def create_final_list_with_date_error(days_period: int) -> (int, list):
    """
    Method that return all dates of the error by turbine
    :param days_period: days backward to be selected in the time series
    :return:id of turbine and list with all date with init and finish
    """
    for (id_turbine, all_dates_by_turbine) in yield_get_all_date_error_by_turbine():
        yield id_turbine, Util_Plt.create_final_list_with_date_error(all_dates_by_turbine, days_period)


def calculus_corr_before_failure_between_nacelle_and_wind_direction(days_period: int) -> None:
    """
    Method that calculates the correlation between the nacelle and the wind direction before the failure happens,
    it selects the amount of information given by days_period
    :param days_period: is the amount of days that selects us by calculation correlation
    :return:
    """
    for (id_turbine, date_to_query) in create_final_list_with_date_error(days_period):
        calculus_correlation_nacelle_wind_direction(date_to_query, id_turbine)


def calculus_correlation_nacelle_wind_direction(date_to_query: list, id_turbine: int):
    if not not date_to_query:
        for value in date_to_query:
            result = db_call.read_nacelle_and_wind_direction(id_turbine, value.date_init, value.date_finish)
            save_all_series_result = [(len(my_list), my_list) for my_list in
                                      Util_Plt.filter_series_by_active_power(result)]
            final_len = search_min_value(save_all_series_result)
            final_element = list(map(lambda value_list: list(map(lambda val: val[0], value_list[1][len(value_list[1]) -
                                                                                                   final_len:])),
                                     save_all_series_result))
            calculus_correlation(final_len, final_element, id_turbine)


def calculus_corr_before_maintenance_between_nacelle_and_wind_direction(days_period: int):
    """
    This method select an interval time between nacelle and wind direction and calculate correlation
    between theirs
    :param days_period: quantity of days that we need select to calculate correlation between series
    :return:
    """
    for (id_turbine, all_dates_by_turbine) in yield_get_all_date_by_turbine_original():
        date_to_query = Util_Plt.create_final_list_with_date_custom(all_dates_by_turbine, days_period)
        calculus_correlation_nacelle_wind_direction(date_to_query, id_turbine)


def search_min_value(save_all_series_result: list):
    return min(list(map(lambda val: val[0], save_all_series_result)), key=lambda x: x)


def calculate_correlation_inside_extra_maintenance():
    """
    Method that calculus correlation between different extra periods maintenance,
    If and only if the extra maintenance is greater that 1
    :return:
    """
    for value_series, id_turbine in get_all_series_maintenance_inside_extra_maintenance_by_turbine():
        if len(value_series) > 1:
            final_len = search_min_value(value_series)
            final_element = list(map(lambda value: list(map(lambda val: val[3], value[1][len(value[1]) - final_len:])),
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
            final_info = Util_Plt.search_warning_after_failure(value_series_single)
            print(f'ID TURBINE {id_turbine} \n')
            for info_by_error in final_info:
                print(f'Error Code {info_by_error.error_code} \n')
                print(f'Total Warning {info_by_error.total_warning} \n')
