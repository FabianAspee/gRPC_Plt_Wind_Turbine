from CaseClassPlt import DateTurbine, TotalWarning, DateTurbineCustom, DateTurbineErrorCustom
from datetime import datetime, timedelta
from functools import reduce
from tail_recursion import tail_recursive, recurse
import numpy as np

errors = [180, 3370, 186, 182, 181]
warnings = [892, 891, 183, 79, 356]
format_date = "%Y/%m/%d %H:%M:%S"


def create_final_list_with_date_custom(all_dates_by_turbine: list, time_interval: int) -> list:
    def _create_final_list_with_date_(all_dates_by_turbine_aux: list, custom_list_own: list) -> list:
        if len(all_dates_by_turbine_aux) >= 0:
            custom_list_own.append(
                DateTurbineCustom((datetime.strptime(all_dates_by_turbine_aux[0].date_init, format_date) -
                                   timedelta(time_interval)).strftime(format_date),
                                  all_dates_by_turbine_aux[0].date_init))
            return _create_final_list_with_date_(all_dates_by_turbine_aux[1:], custom_list_own)
        else:
            return custom_list_own

    custom_list: list = []
    return _create_final_list_with_date_(all_dates_by_turbine, custom_list)


def create_final_list_with_date(all_dates_by_turbine: list) -> list:
    def _create_final_list_with_date_(all_dates_by_turbine_aux: list, custom_list_own: list) -> list:
        if len(all_dates_by_turbine_aux) >= 2:
            custom_list_own.append(DateTurbine(all_dates_by_turbine_aux[0].date_finish,
                                               all_dates_by_turbine_aux[1].date_init, all_dates_by_turbine_aux[0]
                                               .is_normal & all_dates_by_turbine_aux[1].is_normal))
            return _create_final_list_with_date_(all_dates_by_turbine_aux[1:], custom_list)
        elif len(all_dates_by_turbine_aux) == 1:
            custom_list_own.append(
                DateTurbine(all_dates_by_turbine_aux[0].date_finish, datetime.now().strftime(format_date),
                            all_dates_by_turbine_aux[0].is_normal))
            return custom_list_own
        else:
            return custom_list_own

    custom_list: list = []
    if len(all_dates_by_turbine) >= 2:
        custom_list.append(DateTurbine(all_dates_by_turbine[0].date_finish, all_dates_by_turbine[1].date_init,
                                       all_dates_by_turbine[0].is_normal))
        return _create_final_list_with_date_(all_dates_by_turbine[1:], custom_list)
    else:
        return None


def sum_error_period(all_value_period: list):
    number = 0
    for value in all_value_period:
        if value in errors:
            number = number + 1
    return number


def verify_next_50(element: list, index: int, size_period: int, final: list, new_val: list, number_time: int,
                   all_value_period_len: int):
    @tail_recursive
    def __verify_next_50__(element_aux: list, index_aux: int, final: list, new_val_aux: list = None, index_stop=0):
        head, *tail = element_aux
        if len(tail) == 0:
            return element, index, size_period, final
        if head not in errors and index_stop < 50:
            new_val_aux.append(head)
            return recurse(tail, index_aux + 1, final, new_val_aux, index_stop + 1)
        elif index_stop == 50:
            final.append(new_val)
            return element, index, size_period, final
        elif head in errors:
            new_val_aux.append(head)
            final.append(new_val[len(new_val_aux):] + new_val_aux)
            size_period_aux = int(
                (all_value_period_len - index) / (1 if number_time == len(final) else number_time - len(final)))
            return tail, index_aux, size_period_aux, final

    return __verify_next_50__(element, index, final)


def insert_new_value_to_final(new_value: list, val: object, final: list):
    new_value.append(val)
    if len(new_value) > 49:
        final.append(new_value)
    return final


def verify_value_period(all_value_period: list, number_time: int, size_period: int):
    @tail_recursive
    def __verify_value_period__(all_value_period_aux: list, index: int, size_period: int, count_aux: int = 0,
                                new_value: list = None, final: list = None):
        head, *tail = all_value_period_aux
        if len(tail) == 0:
            return insert_new_value_to_final(new_value, head, final)
        if head not in errors and count_aux < size_period and len(tail) > 0:
            new_value.append(head)
            return recurse(tail, index + 1, size_period, count_aux + 1, new_value, final)
        elif count_aux == size_period and len(tail) > 0:
            tail, index, size_period, final = verify_next_50([head] + tail, index, size_period, final, new_value,
                                                             number_time, len(all_value_period))
            return recurse(tail, index + 1, size_period, 1, [], final)
        elif head in errors:
            final = insert_new_value_to_final(new_value, head, final)
            new_value = []
            size_period = int(
                (len(all_value_period) - index) / (1 if number_time == len(final) else number_time - len(final)))
            return recurse(tail, index + 1, size_period, 0, new_value, final)

    return __verify_value_period__(all_value_period, 1, size_period)


def divide_series_same_len(all_value_period: list, number_time: int) -> list:
    all_value = list(map(lambda val: val[3], all_value_period[1]))
    number_error = sum_error_period(all_value)
    number_time = number_error if number_error > number_time else number_time
    value_divide = int(len(all_value_period[1]) / number_time)
    return verify_value_period(all_value, number_time, value_divide)


def split_series_by_error(all_value_period: list):
    sub_series = []
    final_sub_series = []
    for val in all_value_period:
        if val not in errors:
            sub_series.append(val)
        if val in errors:
            sub_series.append(val)
            final_sub_series.append(sub_series)
            sub_series = []
    return final_sub_series


def count_warning_by_sub_series(single_sub_series: list) -> TotalWarning:
    total = 0
    for val in single_sub_series:
        if val in warnings:
            total = total + 1
    return TotalWarning(total, val)


def search_warning_after_failure(all_value_period: list):
    all_value = list(map(lambda val: val[3], all_value_period[1]))
    all_sub_series = split_series_by_error(all_value)
    return [count_warning_by_sub_series(single_sub_series) for single_sub_series in all_sub_series]


def divide_by_week(date_init: datetime, date_finish: datetime, all_value_period: list):
    while date_init <= date_finish:
        date_finish_week = date_init + timedelta(7)
        val_week = [val for val in all_value_period if date_init <= datetime.strptime(val[4], "%Y/%m/%d %H:%M:%S")
                    <= date_finish_week]
        yield val_week
        date_init = date_finish_week


def divide_series_by_week(all_value_period: list) -> list:
    first_date = all_value_period[1][0][4]
    end_date = all_value_period[1][len(all_value_period[1]) - 1][4]
    date_init = datetime.strptime(first_date, "%Y/%m/%d %H:%M:%S")
    date_finish = datetime.strptime(end_date, "%Y/%m/%d %H:%M:%S")

    return [len([res for res in val if res[3] != 'null']) for val in
            divide_by_week(date_init, date_finish, all_value_period[1])]


def create_final_list_with_date_error(all_dates_by_turbine, days_period):
    def _create_final_list_with_date_(all_dates_by_turbine_aux: list, custom_list_own: list) -> list:
        if len(all_dates_by_turbine_aux) > 0:
            custom_list_own.append(
                DateTurbineErrorCustom((datetime.strptime(all_dates_by_turbine_aux[0].date_error, format_date) -
                                        timedelta(days_period)).strftime(format_date),
                                       all_dates_by_turbine_aux[0].date_error,
                                       all_dates_by_turbine_aux[0].error))
            return _create_final_list_with_date_(all_dates_by_turbine_aux[1:], custom_list_own)
        else:
            return custom_list_own

    custom_list: list = []
    return _create_final_list_with_date_(all_dates_by_turbine, custom_list)


def filter_series_by_active_power(all_values: list):
    dictionary = {}
    for val in all_values:
        if val[2] in dictionary:
            dictionary[val[2]].append((val[3], val[4]))
        else:
            dictionary[val[2]] = [(val[3], val[4])]

    for active_power in dictionary[1]:
        if (active_power[0] is None) or (active_power[0] <= 0):
            dictionary[2] = [val for val in dictionary[2] if val[1] != active_power[1]]
            dictionary[4] = [val for val in dictionary[4] if val[1] != active_power[1]]
    return [dictionary[2], dictionary[4]]
