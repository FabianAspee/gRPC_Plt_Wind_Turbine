from matplotlib.colors import LinearSegmentedColormap

import CorrelationPlt as Cr
import HistogramTurbine as Hs
import TypeDistribution as Tp
import AutoCorrelation as Ac
import ChartMaintenance as Ch
import numpy
import matplotlib.pyplot as plt
if __name__ == '__main__':
    # assign data points
    a = numpy.array([[1, 2, 3, 4, 5, 6, 7, 8, 9],
                     [9, 8, 7, 6, 5, 4, 3, 2, 1]])

    # assign categories
    categories = numpy.array([0, 1, 1, 0, 0, 1, 1, 0, 1])

    # assign colors using color codes
    color1 = (0.69411766529083252, 0.3490196168422699,
              0.15686275064945221 )
    color2 = (0.65098041296005249, 0.80784314870834351,
              0.89019608497619629 )

    # asssign colormap
    colormap = numpy.array([color1, color2])
    c_map = LinearSegmentedColormap.from_list("ss", colormap)
    # depict illustration
    scatter = plt.scatter(a[0], a[1], s=500, c=categories,cmap=c_map )
    handles, labels = scatter.legend_elements( prop="colors", alpha=0.3)

    plt.show()



    Ch.chart_maintenance_period_by_turbine_with_warning()
    Ch.chart_maintenance_period_by_turbine_with_angle(10)
    Ac.calculus_auto_correlation_warning_series_before_error(4, 10)
    Cr.calculate_correlation_between_period()
    Cr.calculus_corr_before_failure_between_nacelle_and_wind_direction(3)
    Tp.calculus_qq_plot(15)
    Ac.calculus_auto_correlation_warning_series_before_error_sum_week(10)
    Ac.calculus_auto_correlation_warning_before_error(15, 50)
    Tp.calculus_box_plot(10)
    Tp.calculus_histogram(10)
    Tp.calculus_ks_statistic(10)
    Tp.calculate_shapiro_test(10)
    Hs.histogram_period_maintenance_turbine()
