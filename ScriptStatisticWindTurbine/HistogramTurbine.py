from ReadDB import ReadDB as db
from CaseClassPlt import MaintenanceTurbine, DateTurbineCustom
import UtilsPLT as Util

db_call = db()


def histogram_period_maintenance_turbine():
    initial_maintenance = list(map(lambda values: MaintenanceTurbine(values[0], "", values[1], False),
                                   db_call.read_min_data_series_to_maintenance()))
    all_maintenance: list = list(
        map(lambda values: MaintenanceTurbine(values[1], values[2], values[3], values[4]),
            db_call.read_all_maintenance_turbine()))
    all_difference = Util.calculus_difference_between_dates(
        Util.create_final_list_with_date_turbine(
            [DateTurbineCustom(initial_maintenance[0].date_init, initial_maintenance[0].date_finish)] +
            list(map(lambda value: DateTurbineCustom(value.date_init, value.date_finish), all_maintenance))))
    separate_turbine = separate_series(initial_maintenance + all_maintenance)
    difference_by_turbine = [(key, Util.calculus_difference_between_dates(Util.create_final_list_with_date_turbine(
        list(map(lambda value: DateTurbineCustom(value.date_init, value.date_finish), separate_turbine[key])))))
                             for key in separate_turbine]
    print(all_difference)
    print(difference_by_turbine)


def separate_series(all_maintenance: list):
    dictionary = {}
    for value in all_maintenance:
        if value.id_turbine in dictionary:
            dictionary[value.id_turbine].append(value)
        else:
            dictionary[value.id_turbine] = [value]
    return dictionary
