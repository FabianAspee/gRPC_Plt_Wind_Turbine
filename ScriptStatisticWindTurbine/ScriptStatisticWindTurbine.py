import CorrelationPlt as Cr
import HistogramTurbine as Hs
import TypeDistribution as Tp
if __name__ == '__main__':
    Tp.calculus_ks_statisctic(5)
    Tp.calculus_histogram(5)
    Tp.calculus_box_plot(5)
    Tp.calculus_qq_plot(5)
    Hs.histogram_period_maintenance_turbine()
    Cr.calculus_corr_before_failure_between_nacelle_and_wind_direction(5)
    Cr.calculate_correlation_between_period()

