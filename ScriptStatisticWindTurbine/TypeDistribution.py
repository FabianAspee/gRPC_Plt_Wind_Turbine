from ReadDB import ReadDB as db
import matplotlib.pyplot as plt
import CorrelationPlt as Cr
import UtilsPLT as Util_Plt
import pandas as pd
import numpy as np
import statsmodels.api as sm
import pylab

db_call = db()
__date__ = "date"
__nacelle_direction__ = "nacelle direction"
__wind_direction__ = "wind direction"
__active_power__ = "active power"
__rotor_rpm__ = "rotor rpm"


def read_data(days_period: int):
    for (id_turbine, list_with_date) in Cr.create_final_list_with_date_error(days_period):
        if not not list_with_date:
            for value in list_with_date:
                yield id_turbine, value


def create_data_frame_with_data_filter_by_active_power(days_period: int):
    for (id_turbine, value) in read_data(days_period):
        result = db_call.read_nacelle_and_wind_direction(id_turbine, value.date_init, value.date_finish)
        save_all_series_result = [(len(my_list), my_list) for my_list in
                                  Util_Plt.filter_series_by_active_power(result)]
        yield id_turbine, save_all_series_result


def create_data_frame_with_data(days_period: int):
    for (id_turbine, value) in read_data(days_period):
        yield id_turbine, db_call.read_nacelle_and_wind_direction(id_turbine, value.date_init, value.date_finish)


def group_by_date(series: list, name_series: list) -> dict:
    dictionary = {name_series[0]: name_series[1:]}
    for values in series:
        for element in values[1]:
            if element[1] in dictionary:
                dictionary[element[1]].append(element[0])
            else:
                dictionary[element[1]] = [element[0]]
    return dictionary


def create_np_array(series: list, name_series: list) -> np.array:
    my_dictionary = group_by_date(series, name_series)
    return np.asarray([[key] + my_dictionary[key] for key in my_dictionary])


def calculus_histogram(days_period: int) -> None:
    """
    calculus histogram for a determinate series, we allow see de bell curve distribution
    and determinate if is normal or not normal
    :return:
    """
    for (id_turbine, series) in create_data_frame_with_data_filter_by_active_power(days_period):
        np_array = create_np_array(series, [__date__, __nacelle_direction__, __wind_direction__])
        series_to_plot = pd.DataFrame(np_array[1:, 1:], index=np_array[1:, 0], columns=np_array[0, 1:])
        series_to_plot = series_to_plot.astype({__nacelle_direction__: float, __wind_direction__: float})
        series_to_plot.hist()
        plt.show()


def calculus_box_plot(days_period: int) -> None:
    """
    calculus Box Plot chart that contains 5-number summary of a variable:
    minimum, first quartile, median, third quartile and maximum. this allow see more thing that
    histogram
    :return:
    """
    for (id_turbine, series) in create_data_frame_with_data_filter_by_active_power(days_period):
        np_array = create_np_array(series, [__date__, __nacelle_direction__, __wind_direction__])
        series_to_plot = pd.DataFrame(np_array[1:, 1:], index=np_array[1:, 0], columns=np_array[0, 1:])
        series_to_plot = series_to_plot.astype({__nacelle_direction__: float, __wind_direction__: float})
        series_to_plot.plot(kind='box')
        plt.show()


def calculus_qq_plot(days_period: int) -> None:
    """
    QQ Plot stands for Quantile vs Quantile Plot, which is exactly what it does:
    plotting theoretical quantiles against the actual quantiles of our variable.
    if the data is without the normal quantile, then is not normal
    :return:
    """
    for (id_turbine, series) in create_data_frame_with_data_filter_by_active_power(days_period):
        np_array = create_np_array(series, [__date__, __nacelle_direction__, __wind_direction__])
        series_to_plot = pd.DataFrame(np_array[1:, 1:], index=np_array[1:, 0], columns=np_array[0, 1:])
        sm.qqplot(series_to_plot[__wind_direction__].values.astype(float), line='45')
        sm.qqplot(series_to_plot[__nacelle_direction__].values.astype(float), line='45')
        pylab.show()


def type_distribution_series():
    all_maintenance = db_call.read_all_maintenance_turbine()
    print()
