from ReadDB import ReadDB as db

db_call = db()


def type_distribution_series():
    all_maintenance = db_call.read_all_maintenance_turbine()
    print()