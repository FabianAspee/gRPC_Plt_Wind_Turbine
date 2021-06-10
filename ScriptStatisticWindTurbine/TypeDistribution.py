from pandas import DataFrame

from ReadDB import ReadDB as Db
import matplotlib.pyplot as plt
import CorrelationPlt as Cr
import UtilsPLT as Util_Plt
import pandas as pd
import numpy as np
import statsmodels.api as sm
from scipy.stats import kstest, shapiro, normaltest
import pylab

db_call = Db()

my_custom_array = [Util_Plt.__date__, Util_Plt.__nacelle_direction__, Util_Plt.__wind_direction__]


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
        np_array = create_np_array(series, my_custom_array)
        series_to_plot = pd.DataFrame(np_array[1:, 1:], index=np_array[1:, 0], columns=np_array[0, 1:])
        series_to_plot = series_to_plot.astype({Util_Plt.__nacelle_direction__: float,
                                                Util_Plt.__wind_direction__: float})
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
        np_array = create_np_array(series, my_custom_array)
        series_to_plot = pd.DataFrame(np_array[1:, 1:], index=np_array[1:, 0], columns=np_array[0, 1:])
        series_to_plot = series_to_plot.astype({Util_Plt.__nacelle_direction__: float,
                                                Util_Plt.__wind_direction__: float})
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
        np_array = create_np_array(series, my_custom_array)
        series_to_plot = pd.DataFrame(np_array[1:, 1:], index=np_array[1:, 0], columns=np_array[0, 1:])
        #series_to_plot = pre_processing_min_max_scaler(series_to_plot)
        sm.qqplot(series_to_plot[Util_Plt.__wind_direction__].values.astype(float), line='45')
        sm.qqplot(series_to_plot[Util_Plt.__nacelle_direction__].values.astype(float), line='45')
        pylab.show()
        plt.show()


def calculus_ks_statistic(days_period: int) -> None:
    """
    Kolmogorov Smirnov (KS) Statistic If the observed data perfectly 
    follow a normal distribution, the value of the KS statistic will be 0. 
    The P-Value is used to decide whether the difference is 
    large enough to reject the null hypothesis:
    is the p-value is larger than X value, then we can considerate 
    that this series is normal, in otherwise will be not normal  
    :return:
    """
    for (id_turbine, series) in create_data_frame_with_data_filter_by_active_power(days_period):
        np_array = create_np_array(series, my_custom_array)
        series_to_plot = pd.DataFrame(np_array[1:, 1:], index=np_array[1:, 0], columns=np_array[0, 1:])
        ks_statistic, p_value = kstest(series_to_plot[Util_Plt.__wind_direction__].values.astype(float), 'norm')
        print(f"turbine {id_turbine} ks_statistic {ks_statistic} P-value {p_value}")
        ks_statistic, p_value = kstest(series_to_plot[Util_Plt.__nacelle_direction__].values.astype(float), 'norm')
        print(f"turbine {id_turbine} ks_statistic {ks_statistic} P-value {p_value}")


def calculate_shapiro_test(days_period: int) -> None:
    """
    This method use the shapiro test to identified if series has a normal distribution
     shapiro has been developed specifically for the normal distribution and
     it cannot be used for testing against other distributions like for example
     the KS test.
    :return:
    """
    for (id_turbine, series) in create_data_frame_with_data_filter_by_active_power(days_period):
        np_array = create_np_array(series, my_custom_array)
        series_to_plot = pd.DataFrame(np_array[1:, 1:], index=np_array[1:, 0], columns=np_array[0, 1:])
        shapiro_statistic, p_value = shapiro(series_to_plot[Util_Plt.__wind_direction__].values.astype(float))
        print(f"turbine {id_turbine} shapiro_statistic {shapiro_statistic} P-value {p_value}")
        shapiro_statistic, p_value = shapiro(series_to_plot[Util_Plt.__nacelle_direction__].values.astype(float))
        print(f"turbine {id_turbine} shapiro_statistic {shapiro_statistic} P-value {p_value}")


def calculate_normal_test(days_period: int) -> None:
    """
    This function tests the null hypothesis that a sample comes from a normal distribution.
    It is based on D’Agostino and Pearson’s test that combines skew and kurtosis to produce an omnibus test of
    normality.
    :return:
    """
    for (id_turbine, series) in create_data_frame_with_data_filter_by_active_power(days_period):
        np_array = create_np_array(series, my_custom_array)
        series_to_plot = pd.DataFrame(np_array[1:, 1:], index=np_array[1:, 0], columns=np_array[0, 1:])
        normal_test, p_value = normaltest(series_to_plot[Util_Plt.__wind_direction__].values.astype(float))
        print(f"turbine {id_turbine} normal_test {normal_test} P-value {p_value}")
        shapiro_statistic, p_value = normaltest(series_to_plot[Util_Plt.__nacelle_direction__].values.astype(float))
        print(f"turbine {id_turbine} normal_test {normal_test} P-value {p_value}")


def pre_processing_min_max_scaler(data: DataFrame) -> DataFrame:
    from sklearn.preprocessing import Normalizer
    scaler = Normalizer()
    scaler.fit(data[Util_Plt.__wind_direction__].values.reshape(1, -1))
    data[Util_Plt.__wind_direction__] = scaler.transform(data[Util_Plt.__wind_direction__].values.reshape(1, -1)).reshape(-1,1)
    return data


def type_distribution_series():
    all_maintenance = db_call.read_all_maintenance_turbine()
    print()
