from ReadDB import ReadDB as Db
import matplotlib.pyplot as plt
import TypeDistribution as Tp
import UtilsPLT as Util_Plt
import pandas as pd
import CorrelationPlt as Cr
import numpy as np
from statsmodels.graphics.tsaplots import plot_acf

db_call = Db()

my_custom_array = [Util_Plt.__date__, Util_Plt.__nacelle_direction__, Util_Plt.__wind_direction__]


def calculus_auto_correlation_warning_before_error(days_period: int, lag: int = 10):
    for (id_turbine, series) in Tp.create_data_frame_with_data_filter_by_active_power(days_period):
        np_array = Tp.create_np_array(series, my_custom_array)
        series_to_plot = pd.DataFrame(np_array[1:, 1:], index=np_array[1:, 0], columns=np_array[0, 1:])
        series_to_plot = series_to_plot.astype({Util_Plt.__nacelle_direction__: float,
                                                Util_Plt.__wind_direction__: float})

        plot_acf(series_to_plot[Util_Plt.__wind_direction__].values, lags=lag)
        plot_acf(series_to_plot[Util_Plt.__nacelle_direction__].values, lags=lag)
        plt.show()


def calculus_auto_correlation_warning_series_before_error(number_time: int, lag: int = 5):
    values = ((val, id_turbine) for value_series, id_turbine in Cr.get_all_series_maintenance_by_turbine()
              for val in value_series)
    for value_series, id_turbine in values:
        final_save_all_series_result = Util_Plt.divide_series_same_len(value_series, number_time)
        if not not final_save_all_series_result:
            final = delete_null_value_str(final_save_all_series_result)
            iterator = (list_val for list_val in final if len(list_val) > lag)
            for list_val in iterator:
                plot_acf(list_val, lags=lag)
            plt.show()


def delete_null_value_str(final_series: list) -> list:
    return list(map(lambda list_value: list(filter(lambda value: value != 'null', list_value)),
                    final_series))


def calculus_auto_correlation_warning_series_before_error_sum_week(lag: int = 5):
    for value_series, id_turbine in Cr.get_all_series_maintenance_by_turbine():
        final_series = [Util_Plt.divide_series_by_week(val) for val in value_series]
        final = delete_null_value_str(final_series)
        iterator = (list_val for list_val in final if len(list_val) > lag)
        for list_val in iterator:
            plot_acf(list_val, lags=lag)
        plt.show()
