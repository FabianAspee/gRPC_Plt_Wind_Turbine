from datetime import datetime, timedelta
from typing import List

from CaseClassPlt import DateTurbine, TotalWarning, DateTurbineCustom, DateTurbineErrorCustom
from tail_recursion import tail_recursive, recurse

errors: List[int] = [180, 3370, 186, 182, 181]
warnings: List[int] = [892, 891, 183, 79, 356]
format_date = "%Y/%m/%d %H:%M:%S"
__date__ = "date"
__nacelle_direction__ = "nacelle direction"
__wind_direction__ = "wind direction"
__active_power__ = "active power"
__rotor_rpm__ = "rotor rpm"


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


def create_date_turbine_custom(init: str, finish: str):
    return DateTurbineCustom(init, finish)


def create_final_list_with_date_turbine(all_dates_by_turbine: list) -> list:
    def _create_final_list_with_date_(all_dates_by_turbine_aux: list, custom_list_own: list) -> list:
        if len(all_dates_by_turbine_aux) > 1:
            custom_list_own.append(
                create_date_turbine_custom(all_dates_by_turbine_aux[0].date_finish,
                                           all_dates_by_turbine_aux[1].date_init))
            return _create_final_list_with_date_(all_dates_by_turbine_aux[1:], custom_list_own)
        else:
            custom_list_own.append(
                create_date_turbine_custom(all_dates_by_turbine_aux[0].date_finish,
                                           datetime.now().strftime(format_date)))
            return custom_list_own

    if len(all_dates_by_turbine) > 1:
        custom_list: list = [
            create_date_turbine_custom(all_dates_by_turbine[0].date_finish, all_dates_by_turbine[1].date_init)]
        return _create_final_list_with_date_(all_dates_by_turbine[1:], custom_list)


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

    return __verify_next_50__(element, index, final, new_val_aux=[])


def insert_new_value_to_final(new_value: list, val: object, final: list):
    new_value.append(val)
    if len(new_value) > 49:
        final.append(new_value)
    return final


def verify_value_period(all_value_period: list, number_time: int, size_period: int):
    @tail_recursive
    def __verify_value_period__(all_value_period_aux: list, index: int, size_period_aux: int, count_aux: int = 0,
                                new_value: list = None, final: list = None):
        head, *tail = all_value_period_aux
        if len(tail) == 0:
            return insert_new_value_to_final(new_value, head, final)
        if head not in errors and count_aux < size_period_aux and len(tail) > 0:
            new_value.append(head)
            return recurse(tail, index + 1, size_period_aux, count_aux + 1, new_value, final)
        elif count_aux == size_period_aux and len(tail) > 0:
            tail, index, size_period_aux, final = verify_next_50([head] + tail, index, size_period_aux, final,
                                                                 new_value,
                                                                 number_time, len(all_value_period))
            return recurse(tail, index + 1, size_period_aux, 1, [], final)
        elif head in errors:
            final = insert_new_value_to_final(new_value, head, final)
            new_value = []
            size_period_aux = int(
                (len(all_value_period) - index) / (1 if number_time == len(final) else number_time - len(final)))
            return recurse(tail, index + 1, size_period_aux, 0, new_value, final)

    return __verify_value_period__(all_value_period, 1, size_period, new_value=[], final=[])


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


def verify_date_and_insert(all_dates_by_turbine_aux, days_period: int, custom_list: list):
    if len(custom_list) > 0:
        date_init = datetime.strptime(all_dates_by_turbine_aux.date_error, format_date) - timedelta(days_period)
        end_date = datetime.strptime(custom_list[len(custom_list) - 1].date_finish, format_date)

        return DateTurbineErrorCustom(
            (end_date + timedelta(seconds=10)).strftime(format_date) if date_init < end_date else date_init.strftime(
                format_date), all_dates_by_turbine_aux.date_error,
            all_dates_by_turbine_aux.error)
    else:
        date_init = datetime.strptime(all_dates_by_turbine_aux.date_error, format_date) - timedelta(days_period)
        return DateTurbineErrorCustom(date_init.strftime(format_date), all_dates_by_turbine_aux.date_error,
                                      all_dates_by_turbine_aux.error)


def create_final_list_with_date_error(all_dates_by_turbine, days_period):
    def _create_final_list_with_date_(all_dates_by_turbine_aux: list, custom_list_own: list) -> list:
        if len(all_dates_by_turbine_aux) > 0:
            custom_list_own.append(verify_date_and_insert(all_dates_by_turbine_aux[0], days_period, custom_list_own))
            return _create_final_list_with_date_(all_dates_by_turbine_aux[1:], custom_list_own)
        else:
            return custom_list_own

    custom_list: list = []
    return _create_final_list_with_date_(all_dates_by_turbine, custom_list)


def create_dictionary_by_values(all_values: list):
    dictionary = {}
    for val in all_values:
        if val[2] in dictionary:
            dictionary[val[2]].append((val[3], val[4]))
        else:
            dictionary[val[2]] = [(val[3], val[4])]
    return dictionary


def filter_series_by_active_power(all_values: list):

    dictionary = create_dictionary_by_values(all_values)
    if bool(dictionary):
        for active_power in dictionary[1]:
            if (active_power[0] is None) or (active_power[0] == 'null') or (float(active_power[0]) <= 0):
                dictionary[2] = [val for val in dictionary[2] if val[1] != active_power[1]]
                dictionary[4] = [val for val in dictionary[4] if val[1] != active_power[1]]
        return [dictionary[2], dictionary[4]]
    else:
        return []


def calculus_difference_between_dates(all_dates: List[DateTurbineCustom]):
    return list(map(lambda val: datetime.strptime(val.date_finish, format_date) - datetime.strptime(val.date_init,
                                                                                                    format_date),
                    all_dates)) if not not all_dates else None


def calculus_difference_month_between_dates(all_dates: List[DateTurbineCustom]):
    return list(map(lambda val: (datetime.strptime(val.date_finish, format_date).year -
                                 datetime.strptime(val.date_init, format_date).year) * 12 + (
                                        datetime.strptime(val.date_finish, format_date).month - datetime.strptime(
                                    val.date_init, format_date).month), all_dates)) if not not all_dates else None
