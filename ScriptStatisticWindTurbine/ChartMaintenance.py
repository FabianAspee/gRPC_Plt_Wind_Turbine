from pandas import DataFrame
import UtilsPLT as Util_Plt
from ReadDB import ReadDB as Db
import numpy as np
import matplotlib.pyplot as plt
import CorrelationPlt as Cr
import TypeDistribution as Tp
import matplotlib.dates as mdates
import pandas as pd

db_call = Db()

__total_grade__ = 360


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


def general_plot_custom_date(dates, value, days_period, name_turbine):
    # Load a numpy structured array from yahoo csv data with fields date, open,
    # close, volume, adj_close from the mpl-data/example directory.  This array
    # stores the date as an np.datetime64 with a day unit ('D') in the 'date'
    # column.
    date = np.array([pd.to_datetime(date) for date in dates])
    fig, ax = plt.subplots(figsize=(40, 6))
    ax.plot(date, value)

    # Major ticks every 6 months.
    fmt_half_year = mdates.HourLocator(interval=8)

    create_chart(ax, fmt_half_year, date, fig, name_turbine, "angle_difference")


def chart_maintenance_period_by_turbine_with_angle(days_period: int):
    for (id_turbine, value) in Tp.read_data(days_period):
        name_turbine = db_call.read_name_turbine(id_turbine)[0][0]
        result = db_call.read_nacelle_and_wind_direction(id_turbine, value.date_init, value.date_finish)
        save_all_series_result = [(len(my_list), my_list) for my_list in
                                  Util_Plt.filter_series_by_active_power(result)]
        total_angle = []
        for angle_by_turbine in calculus_angle(save_all_series_result):
            total_angle.append(angle_by_turbine)

        angle = list(map(lambda values: float(values[1]), total_angle))
        date = list(map(lambda values: values[0], total_angle))
        general_plot_custom_date(date, angle, days_period, name_turbine)


def is_int_or_float(date):
    try:
        val = float(date)
        if val != 0.0 and val != np.NaN:
            return True
        else:
            return False
    except:
        return False


def plot_warning(final, id_turbine, name_turbine):
    date = np.array([pd.to_datetime(date[4]) for date in final if is_int_or_float(date[3])])
    fig, ax = plt.subplots(figsize=(40, 6))

    rng = np.random.RandomState(0)
    value_warning = np.array([float(date[3]) for date in final if is_int_or_float(date[3])])
    my_unique_values = list(set(value_warning))
    colors = rng.rand(len(my_unique_values))
    final_element_with_color_and_size = []
    sizes = my_unique_values  # mod size by error, more great the error more great the dimension
    for val in value_warning:
        final_element_with_color_and_size.append(
            (colors[my_unique_values.index(val)], sizes[my_unique_values.index(val)]))

    ax.scatter(date, value_warning, c=list(map(lambda color: color[0], final_element_with_color_and_size)),
               s=list(map(lambda size: size[1], final_element_with_color_and_size)), alpha=0.3, cmap='viridis')
    ax.plot(date, value_warning)

    # Major ticks every 6 months.
    fmt_half_year = mdates.DayLocator(interval=15)
    create_chart(ax, fmt_half_year, date, fig, name_turbine, "maintenance_period")


def create_chart(ax, fmt_half_year, date, fig, name_turbine, directory):
    ax.xaxis.set_major_locator(fmt_half_year)

    # Minor ticks every month.
    fmt_month = mdates.MonthLocator()
    ax.xaxis.set_minor_locator(fmt_month)

    # Text in the x axis will be displayed in 'YYYY-mm' format.
    ax.xaxis.set_major_formatter(mdates.DateFormatter('%Y-%m-%d'))

    # Round to nearest years.
    date_min = np.datetime64(pd.to_datetime(date[0]), 'D')
    date_max = np.datetime64(pd.to_datetime(date[-1]), 'D') + np.timedelta64(1, 'D')
    ax.set_xlim(date_min, date_max)

    # Format the coords message box, i.e. the numbers displayed as the cursor moves
    # across the axes within the interactive GUI.
    ax.format_xdata = mdates.DateFormatter('%Y-%m-%d')
    ax.format_ydata = lambda x: f'${x:.2f}'  # Format the price.
    ax.grid(True)

    # Rotates and right aligns the x labels, and moves the bottom of the
    # axes up to make room for them.
    ax.set_ylabel('Angoli differenza')
    fig.suptitle(f'warning before error')
    fig.autofmt_xdate()
    plt.savefig(f"images/{directory}/turbine-{name_turbine}-period{date_min}-{date_max}")
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
