import CorrelationPlt as Cr
import HistogramTurbine as Hs
if __name__ == '__main__':
    Hs.histogram_period_maintenance_turbine()
    Cr.calculus_corr_before_failure_between_nacelle_and_wind_direction(5)
    Cr.calculate_correlation_between_period()

