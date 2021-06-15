import random
from typing import List

from matplotlib import colors
from matplotlib.colors import LinearSegmentedColormap

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
from CaseClassPlt import ErrorInfo, WarningInfo, DateTurbine

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


color_error = [(255 / 255, 6 / 255, 6 / 255), (122 / 255, 39 / 255, 55 / 255), (158 / 255, 19 / 255, 19 / 255),
               (127 / 255, 17 / 255, 17 / 255), (247 / 255, 52 / 255, 52 / 255)]
dimension_error = [1500, 1400, 1600, 1700, 1800]
color_warning = [(255 / 255, 255 / 255, 0 / 255), (255 / 255, 255 / 255, 112 / 255), (152 / 255, 152 / 255, 8 / 255),
                 (111 / 255, 111 / 255, 9 / 255), (169 / 255, 131 / 255, 6 / 255)]
dimension_warning = [800, 700, 600, 500, 400]
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


def general_plot_custom_date(dates, value, days_period, name_turbine, round_day: int):
    # Load a numpy structured array from yahoo csv data with fields date, open,
    # close, volume, adj_close from the mpl-data/example directory.  This array
    # stores the date as an np.datetime64 with a day unit ('D') in the 'date'
    # column.
    date = np.array([pd.to_datetime(date) for date in dates])
    fig, ax = plt.subplots(figsize=(40, 6))
    ax.plot(date, value)

    # Major ticks every 6 months.
    fmt_half_year = mdates.HourLocator(interval=8)

    create_chart(ax, fmt_half_year, date, fig, name_turbine, "angle_difference", 'Angoli differenza', round_day)


def chart_maintenance_period_by_turbine_with_angle(days_period: int, round_day: int):
    for (id_turbine, value) in Tp.read_data(days_period):
        name_turbine = db_call.read_name_turbine(id_turbine)[0][0]
        result = db_call.read_nacelle_and_wind_direction(id_turbine, value.date_init, value.date_finish)
        save_all_series_result = [(len(my_list), my_list) for my_list in
                                  Util_Plt.filter_series_by_active_power(result)]
        total_angle = []
        print(value)
        for angle_by_turbine in calculus_angle(save_all_series_result):
            total_angle.append(angle_by_turbine)

        angle = list(map(lambda values: float(values[1]), total_angle))
        date = list(map(lambda values: values[0], total_angle))
        general_plot_custom_date(date, angle, days_period, name_turbine, round_day)


def is_int_or_float(date):
    try:
        val = float(date)
        if val != 0.0 and val != np.NaN:
            return True
        else:
            return False
    except:
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
            result = (random.random(), random.random(), random.random())
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


def plot_warning(final, id_turbine, name_turbine):
    date = np.array([pd.to_datetime(date[4]) for date in final if is_int_or_float(date[3])])
    fig, ax = plt.subplots(figsize=(40, 6))

    values_warning = np.array([float(date[3]) for date in final if is_int_or_float(date[3])])
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

    c_map = LinearSegmentedColormap.from_list(c_map_name, [total_color[index] for index in
                                                           my_unique_colors])  # set new array with new index by color
    scatter = ax.scatter(date, values_warning, c=color_index,
                         s=list(map(lambda size: size[1], final_element_with_color_and_size)), alpha=0.3, cmap=c_map,
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
    create_chart(ax, fmt_half_year, date, fig, name_turbine, "maintenance_period", "warning and error")


def create_chart(ax, fmt_half_year, date, fig, name_turbine, directory, y_label, round_day: int = 2):
    ax.xaxis.set_major_locator(fmt_half_year)

    # Minor ticks every month.
    fmt_month = mdates.MonthLocator()
    ax.xaxis.set_minor_locator(fmt_month)

    # Text in the x axis will be displayed in 'YYYY-mm' format.
    ax.xaxis.set_major_formatter(mdates.DateFormatter('%Y-%m-%d'))

    # Round to nearest years.
    date_min = np.datetime64(pd.to_datetime(date[0]), 'D') - np.timedelta64(round_day, 'D')
    date_max = np.datetime64(pd.to_datetime(date[-1]), 'D') + np.timedelta64(round_day, 'D')
    ax.set_xlim(date_min, date_max)

    # Format the coords message box, i.e. the numbers displayed as the cursor moves
    # across the axes within the interactive GUI.
    ax.format_xdata = mdates.DateFormatter('%Y-%m-%d')
    ax.format_ydata = lambda x: f'${x:.2f}'  # Format the price.
    ax.grid(True)

    # Rotates and right aligns the x labels, and moves the bottom of the
    # axes up to make room for them.
    ax.set_ylabel(y_label)
    fig.suptitle(f'warning before error')
    fig.autofmt_xdate()
    plt.savefig(f"images/{directory}/turbine-{name_turbine}-period-{date_min}-{date_max}")
    plt.show()


def chart_maintenance_period_by_turbine_with_warning():
    """
    method that create chart with all warning inside maintenance period
    :return:
    """
    for (id_turbine, all_dates_by_turbine) in Cr.yield_get_all_date_by_turbine_with_final():
        date_to_query = Util_Plt.create_final_list_with_date(list(
            filter(lambda values: values.is_normal, all_dates_by_turbine)))
        name_turbine = db_call.read_name_turbine(id_turbine)[0][0]
        if date_to_query is not None:
            save_all_series_result: list = []
            for value in date_to_query:
                result = db_call.read_warning_and_error_turbine(id_turbine, value.date_init, value.date_finish)
                save_all_series_result.append((len(result), result))

            final = list(filter(lambda filter_value: filter_value[0] != 0, save_all_series_result))
            for value_final in final:
                plot_warning(value_final[1], id_turbine, name_turbine)


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
            print(id_turbine, "not contains maintenance")
            print(custom_date)

    pd_histogram = pd.DataFrame(final_mont, columns=["month_difference"])
    pd_histogram.hist(legend=True)
