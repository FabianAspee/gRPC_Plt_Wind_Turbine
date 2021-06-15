from matplotlib.colors import LinearSegmentedColormap

import CorrelationPlt as Cr
import HistogramTurbine as Hs
import TypeDistribution as Tp
import AutoCorrelation as Ac
import ChartMaintenance as Ch
import numpy
import matplotlib.pyplot as plt
if __name__ == '__main__':
   import numpy as np
   import matplotlib.pyplot as plt
   from matplotlib import cm
   from matplotlib.colors import ListedColormap, LinearSegmentedColormap
   viridis = cm.get_cmap('viridis', 256)
   newcolors = viridis(np.linspace(0, 1, 256))
   pink = np.array([248 / 256, 24 / 256, 148 / 256, 1])
   newcolors[:25, :] = pink
   newcmp = ListedColormap(newcolors)


   Ch.chart_maintenance_period_by_turbine_with_defined_warning()
   Ch.chart_histogram_maintenance_with()
   #Ch.chart_maintenance_period_by_turbine_with_angle(10, 1)
    #Ch.chart_histogram_maintenance()
   Ch.chart_maintenance_period_by_turbine_with_warning()
   """Ac.calculus_auto_correlation_warning_series_before_error(4, 10)
   Cr.calculate_correlation_between_period()
   Cr.calculus_corr_before_failure_between_nacelle_and_wind_direction(3)
   Tp.calculus_qq_plot(15)
   Ac.calculus_auto_correlation_warning_series_before_error_sum_week(10)
   Ac.calculus_auto_correlation_warning_before_error(15, 50)
   Tp.calculus_box_plot(10)
   Tp.calculus_histogram(10)
   Tp.calculus_ks_statistic(10)
   Tp.calculate_shapiro_test(10)
   Hs.histogram_period_maintenance_turbine()"""
