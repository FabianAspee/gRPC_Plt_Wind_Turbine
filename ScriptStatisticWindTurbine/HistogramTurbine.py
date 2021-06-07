from ReadDB import ReadDB as db

db_call = db()


def histogram_period_maintenance_turbine():
    all_maintenance = db_call.read_all_maintenance_turbine()
