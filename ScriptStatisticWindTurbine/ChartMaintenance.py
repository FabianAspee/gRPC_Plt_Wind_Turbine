import calendar
import random
from datetime import datetime
from typing import List

from matplotlib import colors
from matplotlib.colors import LinearSegmentedColormap

from matplotlib import collections as matcoll
from tail_recursion import tail_recursive, recurse
from pandas import DataFrame
import UtilsPLT as Util_Plt
from ReadDB import ReadDB as Db
import numpy as np
import matplotlib.pyplot as plt
import CorrelationPlt as Cr
import TypeDistribution as Tp
import matplotlib.dates as mdates
import pandas as pd
from CaseClassPlt import ErrorInfo, WarningInfo, DateTurbine, InfoTurbine, InfoTurbineAll

db_call = Db()


def load_color_errors():
    return [ErrorInfo(error[0], error[1], error[2] + error[0]) for error in zip(Util_Plt.errors, color_error,
                                                                                dimension_error)]


def load_color_warnings():
    return [WarningInfo(warning[0], warning[1], warning[2]) for warning in
            zip(Util_Plt.warnings, color_warning, dimension_warning)]


__total_grade__ = 360


def get_int_from_rgb(rgb):
    red = rgb[0]
    green = rgb[1]
    blue = rgb[2]
    print(red, green, blue)
    rgb_int = (red << 16) + (green << 8) + blue
    return rgb_int


color_error = [(255 / 255, 0 / 255, 0 / 255, 1), (178 / 255, 1 / 255, 1 / 255, 1), (158 / 255, 19 / 255, 19 / 255, 1),
               (127 / 255, 17 / 255, 17 / 255, 1), (247 / 255, 52 / 255, 52 / 255, 1)]
dimension_error = [1500, 1400, 1600, 1700, 1800]
color_warning = [(255 / 255, 255 / 255, 0 / 255, 1), (255 / 255, 255 / 255, 112 / 255, 1),
                 (152 / 255, 152 / 255, 8 / 255, 1),
                 (111 / 255, 111 / 255, 9 / 255, 1), (169 / 255, 131 / 255, 6 / 255, 1)]
dimension_warning = [100, 200, 800, 900, 500]
errors = load_color_errors()
warnings = load_color_warnings()
aux_errors = Util_Plt.errors
aux_warning = Util_Plt.warnings


def create_dictionary_by_values(all_values: list):
    dictionary = {}
    for list_value in all_values:
        for val in list_value:
            if val[1] in dictionary:
                dictionary[val[1]].append(val[0])
            else:
                dictionary[val[1]] = [val[0]]
    return dictionary


def calculus_angle(save_all_series_result):
    save_all_series_result = list(map(lambda val: val[1], save_all_series_result))
    dictionary = create_dictionary_by_values(save_all_series_result)
    for first in dictionary:
        first_val, second_val = dictionary[first]
        yield ((first, (__total_grade__ - abs(float(first_val) - float(second_val)))) if abs(
            float(first_val) - float(second_val)) > 180 else (first, abs(float(first_val) - float(second_val)))) \
            if first_val != 'null' and second_val != 'null' and first_val is not None and second_val is not None else \
            (first, np.NaN)


def general_plot_custom_date(dates, value, days_period, name_turbine, round_day: int, error):
    # Load a numpy structured array from yahoo csv data with fields date, open,
    # close, volume, adj_close from the mpl-data/example directory.  This array
    # stores the date as an np.datetime64 with a day unit ('D') in the 'date'
    # column.
    date = np.array([pd.to_datetime(date) for date in dates])
    fig, ax = plt.subplots(figsize=(40, 6))
    plt.ylim(0, 180)
    ax.plot(date, value)

    # Major ticks every 6 months.
    fmt_half_year = mdates.HourLocator(interval=8)
    create_chart(ax, fmt_half_year, date, fig, name_turbine, "angle_difference", 'Angoli differenza',
                 f"Differenza between nacelle direction e wind direction prima del errore {error}", round_day)


def chart_event_and_angle_by_period_maintenance():
    for (id_turbine, all_dates_by_turbine, name_turbine, all_dates_of_period) in chart_maintenance():
        for value in all_dates_of_period:
            for angle, date in get_angle_by_date(id_turbine, value):
                print()


def get_info(id_turbine: int, value):
    result = db_call.read_nacelle_and_wind_direction(id_turbine, value.date_init, value.date_finish)
    #print(value)
    save_all_series_result = [(len(my_list), my_list) for my_list in Util_Plt.filter_series_by_active_power(result)
                              if len(my_list) > 0]
    if len(save_all_series_result) > 0:
        total_angle = []
        for angle_by_turbine in calculus_angle(save_all_series_result):
            total_angle.append(angle_by_turbine)
        yield total_angle, result[0][1]


def get_angle(id_turbine: int, value):
    for total_angle, id_sensor in get_info(id_turbine, value):
        angle = list(map(lambda values: float(values[1]), total_angle))
        yield angle, id_sensor


def get_angle_by_date(id_turbine: int, value):
    for total_angle, _ in get_info(id_turbine, value):
        angle = list(map(lambda values: float(values[1]), total_angle))
        date = list(map(lambda values: values[0], total_angle))
        yield angle, date


def get_info_to_chart_angle(days_period: int):
    for (id_turbine, value) in Tp.read_data(days_period):
        name_turbine = db_call.read_name_turbine(id_turbine)[0][0]
        angle, date = get_angle_by_date(id_turbine, value)
        yield id_turbine, name_turbine, angle, date, value


def plot_event_and_angle(final, id_turbine, name_turbine, function, name_chart, angle, date_angle):
    date = np.array([pd.to_datetime(date[4]) for date in final if function(date[3])])
    if len(date) > 0:
        values_warning = np.array([float(date[3]) for date in final if function(date[3])])
        fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(40, 15))
        fig.tight_layout(pad=5.0)
        fig.suptitle(f'Information event and angle turbine {name_turbine}')

        fmt_half_year = mdates.HourLocator(interval=8)
        ax1.plot(date, values_warning, 'o-')
        ax1.set_ylabel('event')
        ax1 = set_axis_dates(ax1, fmt_half_year, date_angle)
        ax2.plot(date_angle, angle, '.-')
        ax2.set_xlabel('time (s)')
        ax2.set_ylabel('angle when generate energy')
        ax2 = set_axis_dates(ax2, fmt_half_year, date_angle)
        # Rotates and right aligns the x labels, and moves the bottom of the
        # axes up to make room for them.
        date_min, date_max = get_date_min_max(date)
        rotate_x_axis(ax1)
        rotate_x_axis(ax2)
        plt.savefig(f"images/{name_chart}/turbine-{name_turbine}-period-{date_min}-{date_max}")
        plt.show()


def get_date_min_max(date, round_day: int = 2):
    return np.datetime64(pd.to_datetime(date[0]), 'D') - np.timedelta64(round_day, 'D'), \
           np.datetime64(pd.to_datetime(date[-1]), 'D') + np.timedelta64(round_day, 'D')


def set_axis_dates(axis, fmt_half_year, date, round_day: int = 2):
    axis.xaxis.set_major_locator(fmt_half_year)

    # Minor ticks every month.
    fmt_month = mdates.MonthLocator()
    axis.xaxis.set_minor_locator(fmt_month)

    # Text in the x axis will be displayed in 'YYYY-mm' format.
    axis.xaxis.set_major_formatter(mdates.DateFormatter('%Y-%m-%d'))

    # Round to nearest years.
    date_min, date_max = get_date_min_max(date, round_day)
    axis.set_xlim(date_min, date_max)

    # Format the coords message box, i.e. the numbers displayed as the cursor moves
    # across the axes within the interactive GUI.
    axis.format_xdata = mdates.DateFormatter('%Y-%m-%d')
    axis.format_ydata = lambda x: f'${x:.2f}'  # Format the price.
    axis.grid(True)
    return axis


def rotate_x_axis(axis):
    for tick in axis.get_xticklabels():
        tick.set_rotation(45)


def chart_event_and_angle(days_period: int):
    for (id_turbine, name_turbine, angle, date, value) in get_info_to_chart_angle(days_period):
        for final in get_read_warning_and_error_period([value], id_turbine):
            for value_final in final:
                plot_event_and_angle(value_final[1], id_turbine, name_turbine, is_int_or_float_unique_warning,
                                     "angle_and_event", angle, [pd.to_datetime(date_value) for date_value in date])


def chart_maintenance_period_by_turbine_with_angle(days_period: int, round_day: int):
    for (id_turbine, name_turbine, angle, date, value) in get_info_to_chart_angle(days_period):
        general_plot_custom_date(date, angle, days_period, name_turbine, round_day, value.error)


def is_int_or_float(date):
    try:
        val = float(date)
        if val != 0.0 and val != np.NaN:
            return True
        else:
            return False
    except ValueError:
        return False


def is_int_or_float_unique_warning(date):
    try:
        val = float(date)
        if val != 0.0 and val != np.NaN and (val in aux_warning or val in aux_errors):
            return True
        else:
            return False
    except ValueError:
        return False


def filter_color(value: float):
    return filter_color_error(value) if value in aux_errors else filter_color_warning(value)


def filter_dimension(value: float):
    return filter_dimension_error(value) if value in aux_errors else filter_dimension_warning(value)


def filter_color_error(value: float):
    return list(filter(lambda value_error: value_error.error == value, errors))[0].color


def filter_color_warning(value: float):
    return list(filter(lambda value_warning: value_warning.warning == value, warnings))[0].color


def filter_dimension_error(value: float):
    return list(filter(lambda value_error: value_error.error == value, errors))[0].dimension


def filter_dimension_warning(value: float):
    return list(filter(lambda value_warning: value_warning.warning == value, warnings))[0].dimension


def create_color(my_unique_values) -> list:
    @tail_recursive
    def _create_color_(unique_values, my_colors: list = None):
        if len(unique_values) > 0:
            result = (random.random(), random.random(), random.random(), 1)
            if result not in color_error and color_warning:
                my_colors.append(result)
                return recurse(unique_values[1:], my_colors)
            else:
                return recurse(unique_values, my_colors)
        else:
            return my_colors

    return _create_color_(my_unique_values, [])


def change_string(label: str, error: float):
    label_f = label[:label.find('{') + 1] + str(error) + "}$"
    return label_f


def change_strings(labels: list, final_list_error: np.array):
    return [change_string(label, error) for label, error in zip(labels, final_list_error)]


def condition_without_error_and_warning(value_warning, my_unique_values: list):
    return True if value_warning not in my_unique_values and value_warning not in aux_errors \
                   and value_warning not in aux_warning else False


def condition_with_error_and_warning(value_warning, my_unique_values: list):
    return False if value_warning in my_unique_values else True


def get_unique_values(values_warning: np.array, condition) -> list:
    my_unique_values = []
    for value_warning in values_warning:
        if condition(value_warning, my_unique_values):
            my_unique_values.append(value_warning)
    return my_unique_values


def get_final_element_color_and_size(my_unique_values: list, values_warning: np.array, colors_custom: list,
                                     sizes: list) -> list:
    final_element_with_color_and_size = []
    for val in values_warning:
        if val in my_unique_values:
            size = sizes[my_unique_values.index(val)]
            final_element_with_color_and_size.append(
                (colors_custom[my_unique_values.index(val)], size if size < 300 else 300))
        else:
            final_element_with_color_and_size.append(
                (filter_color(val), filter_dimension(val)))
    return final_element_with_color_and_size


def plot_warning(final, id_turbine, name_turbine, function, directory):
    date = np.array([pd.to_datetime(date[4]) for date in final if function(date[3])])
    if len(date) > 0:
        fig, ax = plt.subplots(figsize=(40, 6))

        values_warning = np.array([float(date[3]) for date in final if function(date[3])])
        my_unique_values = get_unique_values(values_warning, condition_without_error_and_warning)
        colors_custom = create_color(my_unique_values)
        sizes = my_unique_values
        final_element_with_color_and_size = get_final_element_color_and_size(my_unique_values, values_warning,
                                                                             colors_custom, sizes)

        final_colors = list(map(lambda color: color[0], final_element_with_color_and_size))

        c_map_name = 'custom_color_map'
        total_color = colors_custom + color_error + color_warning
        final_list_color_unique = []
        final_index = []
        for index, color_l in enumerate(final_colors, start=0):
            if color_l not in final_list_color_unique:
                final_list_color_unique.append(color_l)
                final_index.append(index)
        color_index = np.array([[final_list_color_unique.index(color), total_color.index(color)] for color in
                                final_colors])

        my_unique_colors = []
        color_index_aux = color_index[:, 1]
        color_index = color_index[:, 0]
        for value_color in color_index_aux:
            if value_color not in my_unique_colors:
                my_unique_colors.append(value_color)
        color_list = [total_color[index] for index in my_unique_colors]
        if len(color_list) == 1:
            color_list.append(color_warning[0])
        c_map = LinearSegmentedColormap.from_list(c_map_name, color_list)  # set new array with new index by color
        scatter = ax.scatter(date, values_warning, c=color_index,
                             s=list(map(lambda size: size[1], final_element_with_color_and_size)), alpha=0.3,
                             cmap=c_map,
                             label=len(my_unique_colors))
        handles, labels = scatter.legend_elements(num=len(my_unique_colors), prop="colors", alpha=0.3)

        labels = change_strings(labels, get_unique_values(values_warning, condition_with_error_and_warning))
        n_col = int(len(my_unique_colors) / 2)
        legend1 = ax.legend(handles, labels, loc='upper center', ncol=n_col, mode="expand", shadow=True, fancybox=True,
                            title="Errors")
        legend1.get_frame().set_alpha(0.1)
        ax.add_artist(legend1)
        ax.plot(date, values_warning)
        fmt_half_year = mdates.DayLocator(interval=15)
        create_chart(ax, fmt_half_year, date, fig, name_turbine, directory, "warning and error",
                     'warning before error')


def create_chart(ax, fmt_half_year, date, fig, name_turbine, directory, y_label, title: str, round_day: int = 2):
    ax = set_axis_dates(ax, fmt_half_year, date, round_day)
    date_min, date_max = get_date_min_max(date, round_day)
    # Rotates and right aligns the x labels, and moves the bottom of the
    # axes up to make room for them.
    ax.set_ylabel(y_label)
    fig.suptitle(title)
    fig.autofmt_xdate()
    plt.savefig(f"images/{directory}/turbine-{name_turbine}-period-{date_min}-{date_max}")
    plt.show()


def chart_maintenance_period_by_turbine_with_warning():
    """
    method that create chart with all warning inside maintenance period
    :return:
    """
    for (id_turbine, all_dates_by_turbine, name_turbine, _) in chart_maintenance():
        for value_final in all_dates_by_turbine:
            plot_warning(value_final[1], id_turbine, name_turbine, is_int_or_float, "maintenance_period")


def chart_histogram_maintenance():
    """
    Selects all normal maintenance period dates by turbine, adds the initial period, i.e.,
    the initial date of when the information for the series begins to exist
    :return:
    """
    final_mont = []
    for (id_turbine,) in db_call.read_id_turbine():
        normal_maintenance = list(map(lambda value: DateTurbine("", value[4], False),
                                      db_call.read_min_data_series(id_turbine))) + list(
            map(lambda value: DateTurbine(value[2], value[3], bool(value[4])),
                db_call.read_normal_maintenance(id_turbine)))
        custom_date = Util_Plt.create_final_list_with_date_turbine(normal_maintenance)
        if custom_date is not None:
            print(id_turbine)
            final = Util_Plt.calculus_difference_month_between_dates(custom_date[:len(custom_date) - 1])
            final_mont.extend(final)
        else:
            print(id_turbine, "not contains normal maintenance")
            print(custom_date)

    pd_histogram = pd.DataFrame(final_mont, columns=["month_difference"])
    pd_histogram.hist(legend=True)
    plt.savefig(f"images/histogram/histogram_turbine_maintenance")
    plt.show()


def chart_histogram_maintenance_with():
    """
    Selects all normal maintenance period dates by turbine, adds the initial period, i.e.,
    the initial date of when the information for the series begins to exist
    :return:
    """
    final_mont = []
    for (id_turbine, date_maintenance) in Cr.yield_get_all_date_by_turbine_with_final():
        custom_date = Util_Plt.create_final_list_with_date_turbine(date_maintenance)
        if custom_date is not None:
            print(id_turbine)
            final = Util_Plt.calculus_difference_month_between_dates(custom_date[:len(custom_date) - 1])
            final_mont.extend(final)
        else:
            print(id_turbine, "not contains normal maintenance")
            print(custom_date)

    pd_histogram = pd.DataFrame(final_mont, columns=["month_difference"])
    pd_histogram.hist(legend=True)
    plt.savefig(f"images/histogram/histogram_turbine_maintenance_extra")
    plt.show()


def get_read_warning_and_error_period(date_to_query, id_turbine):
    save_all_series_result: list = []
    for value in date_to_query:
        result = db_call.read_warning_and_error_turbine(id_turbine, value.date_init, value.date_finish)
        save_all_series_result.append((len(result), result))

    final = list(filter(lambda filter_value: filter_value[0] != 0, save_all_series_result))
    yield final


def chart_maintenance():
    for (id_turbine, all_dates_by_turbine) in Cr.yield_get_all_date_by_turbine_with_final():
        date_to_query = Util_Plt.create_final_list_with_date(list(
            filter(lambda values: values.is_normal, all_dates_by_turbine)))
        name_turbine = db_call.read_name_turbine(id_turbine)[0][0]
        if date_to_query is not None:
            for final in get_read_warning_and_error_period(date_to_query, id_turbine):
                yield id_turbine, final, name_turbine, date_to_query


def chart_maintenance_period_by_turbine_with_defined_warning():
    """
    method that create chart with all warning inside maintenance period
    :return:
    """
    for (id_turbine, all_dates_by_turbine, name_turbine, _) in chart_maintenance():
        for value_final in all_dates_by_turbine:
            plot_warning(value_final[1], id_turbine, name_turbine, is_int_or_float_unique_warning,
                         "maintenance_with_unique_warning")


def get_value_event():
    for (id_turbine, all_dates_by_turbine, name_turbine, _) in chart_maintenance():
        flatten_list = [val for value_final in all_dates_by_turbine for val in value_final[1]]

        name_turbine = db_call.read_name_turbine(id_turbine)[0][0]
        date_and_value = [(pd.to_datetime(date[4]), date[3]) for date in flatten_list
                          if is_int_or_float_unique_warning(date[3])]
        min_and_max_date = Cr.get_min_and_max_date(id_turbine)
        all_month = Util_Plt.create_all_month_name(min_and_max_date)
        all_maintenance = Cr.get_all_init_date_by_turbine(id_turbine)
        total_info, lines_aux = Util_Plt.aggregate_event_by_month(all_month, date_and_value, all_maintenance)

        date = [date[0] for date in total_info]
        total_warning = [warning[1] for warning in total_info]
        size = [(warning[1] * 200) + 100 for warning in total_info]
        max_warning = max(total_warning)
        yield name_turbine, date, size, max_warning, id_turbine, lines_aux, total_warning


def chart_maintenance_aggregate():
    for (name_turbine, date, size, max_warning, id_turbine, lines_aux, total_warning) in get_value_event():
        fig, ax = plt.subplots(figsize=(40, 6))
        lines = [[(i, 0), (i, max_warning + 1)] if lines_aux[i][0] == 1 else [(i, 0), (i, 0)] for i in
                 range(len(lines_aux))]
        plt.ylim(0, max_warning + 1)
        line_coll = matcoll.LineCollection(lines)
        ax.scatter(date, total_warning, alpha=0.3)
        [ax.text(x, int(max_warning / 2), "Manutenzione normale" if l_aux[0] == 1 else "Manutenzione straordinaria",
                 rotation=90, verticalalignment="bottom")
         for (x, l_aux) in zip(date, lines_aux) if l_aux[0] == 1]

        plt.plot(date, total_warning)
        ax.add_collection(line_coll)
        fig.suptitle(f"Error by month {name_turbine}")
        fig.autofmt_xdate()
        plt.savefig(
            f"images/maintenance_aggregate/turbine-{name_turbine}-period-{defined_format(date[0])}-{defined_format(date[-1])}")
        plt.show()


def defined_format(date):
    result = check_format_date(date)
    return calendar.month_name[result.month] + "-" + str(result.year)


def check_format_date(date):
    try:
        return datetime.strptime(date, "%B-%Y")
    except:
        return datetime.strptime(date, "%B-%Y-%d %H:%M")


def create_dictionary_warning_by_month(dates: list, dictionary: dict, total_warning: list, name_turbine: str,
                                       lines_aux: list):
    # print(f'dates {len(dates)} warning {len(total_warning)}')
    for (date, warning, type_maintenance) in zip(dates, total_warning, lines_aux):
        if date in dictionary:
            dictionary[date].append((warning if type_maintenance == (0, False) else 0, name_turbine, type_maintenance))

        else:
            dictionary[date] = [(warning if type_maintenance == (0, False) else 0, name_turbine, type_maintenance)]
    return dictionary


def check_format(value, final_total_warning_aux):
    index = final_total_warning_aux.index(value)
    if not value.maintenance and index < len(final_total_warning_aux) - 1 and \
            not not final_total_warning_aux[index + 1].maintenance:
        return calendar.month_name[value.date.month] + "-" + str(value.date.year) + "-" + str(
            value.date.day) + " " + str(value.date.hour) + ":" + str(value.date.minute)
    if not value.maintenance and index > 0 and not not final_total_warning_aux[index + 0].maintenance:
        return calendar.month_name[value.date.month] + "-" + str(value.date.year) + "-" + str(
            value.date.day) + " " + str(value.date.hour) + ":" + str(value.date.minute)
    if not not value.maintenance:
        return calendar.month_name[value.date.month] + "-" + str(value.date.year) + "-" + str(
            value.date.day) + " " + str(value.date.hour) + ":" + str(value.date.minute)
    else:
        return calendar.month_name[value.date.month] + "-" + str(value.date.year)


def set_text_maintenance(info: InfoTurbineAll):
    normal = []
    extra = []
    for val in info.maintenance:
        if val[2] == (1, True):
            normal.append(val[1])
        else:
            extra.append(val[1])
    return " ".join(["Normal Maintenance" if not not normal else "", ' '.join(normal) if not not normal else "",
                     "Extra Maintenance" if not not extra else "", ' '.join(extra) if not not extra else ""])


def chart_maintenance_aggregate_all_turbine_event_month():
    info_turbines = []
    final_warning = {}
    date = []
    all_names = [db_call.read_name_turbine(id_turbine)[0][0] for (id_turbine,) in db_call.read_id_turbine()]
    for (name_turbine, date, size, max_warning, id_turbine, lines_aux, total_warning) in get_value_event():
        date = date
        info_turbines.append(InfoTurbine(lines_aux, date, name_turbine))
        final_warning = create_dictionary_warning_by_month(date, final_warning, total_warning, name_turbine, lines_aux)
        print(f"final warning {len(final_warning)}")
    final_total_warning = list(map(lambda key: InfoTurbineAll(check_format_date(key),
                                                              sum(list(map(lambda value_sum: value_sum[0], filter(
                                                                  lambda value_list: value_list[2] == (0, False),
                                                                  final_warning[key])))),
                                                              list(filter(lambda value_list: value_list[2] == (1, True)
                                                                                             or value_list[2] == (
                                                                                                 1, False),
                                                                          final_warning[key]))),
                                   final_warning))
    fig, ax = plt.subplots(figsize=(40, 6))
    final_total_warning = sorted(final_total_warning, key=lambda info_turbine: info_turbine.date)
    max_warning = max(final_total_warning, key=lambda info_turbine: info_turbine.qta_warning).qta_warning
    lines = [[(i, 0), (i, max_warning)] if not not final_total_warning[i].maintenance else [(i, 0), (i, 0)]
             for i in range(len(final_total_warning))]
    plt.ylim(0, max_warning)
    line_coll = matcoll.LineCollection(lines)

    dates = list(map(lambda value: check_format(value, final_total_warning), final_total_warning))
    event = list(map(lambda value: value.qta_warning, final_total_warning))
    ax.scatter(dates, event, alpha=0.3)
    [ax.text(check_format(info, final_total_warning), int(max_warning / (10 if max_warning >= 100 else 5)),
             set_text_maintenance(info),
             rotation=90, horizontalalignment='left', verticalalignment='bottom') for index, info in
     enumerate(final_total_warning, start=0) if not not info.maintenance]
    plt.plot(dates, event)
    ax.add_collection(line_coll)
    fig.suptitle(f"Error by month {'-'.join(all_names)}")
    fig.autofmt_xdate()
    plt.savefig(f"images/maintenance_aggregate_total/all-turbine-period-{date[0]}-{date[-1]}")
    plt.show()
