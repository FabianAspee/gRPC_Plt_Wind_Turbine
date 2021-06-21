import calendar
import random
from datetime import datetime
from typing import List

from matplotlib import colors
from matplotlib.colors import LinearSegmentedColormap

from matplotlib import collections as matcoll
from tail_recursion import tail_recursive, recurse
from pandas import DataFrame
import UtilsPLT as Util_Plt
from ReadDB import ReadDB as Db
import numpy as np
import ChartMaintenance as Ch
import matplotlib.pyplot as plt
import CorrelationPlt as Cr
import TypeDistribution as Tp
import matplotlib.dates as mdates
import pandas as pd
from CaseClassPlt import ErrorInfo, WarningInfo, DateTurbine, InfoTurbine, InfoTurbineAll, DateTurbineError, \
    SensorInformation

db_call = Db()


def yield_get_all_date_event_by_turbine():
    """
    Select all date where an error happened
    :return:list with date of the turbine when error occurred
    """
    for (id_turbine,) in db_call.read_id_turbine():
        yield id_turbine, list(map(lambda value: DateTurbineError(value[4], np.nan),
                                   db_call.read_min_data_series(id_turbine))) + list(
            map(lambda value: DateTurbineError(value[4], value[3]), db_call.read_data_event_turbines(id_turbine)))


def print_angle_all_turbine(info_by_turbine: dict):
    fig, axis = plt.subplots(2, 1, figsize=(40, 15))
    fig.tight_layout(pad=5.0)
    fig.suptitle(f'Angle turbine between errors')
    key_dictionary = []
    for key in info_by_turbine:
        if len(key_dictionary) < 2:
            key_dictionary.append(key)
    for index, key in zip(range(len(axis)), key_dictionary):
        for val in info_by_turbine[key]:
            axis[index].plot([i for i in range(len(val))], val)
        axis[index].set_ylabel('event')
        # Rotates and right aligns the x labels, and moves the bottom of the
        # axes up to make room for them.
        Ch.rotate_x_axis(axis[index])
    # plt.savefig(f"images/{name_chart}/turbine-{name_turbine}-period-{date_min}-{date_max}")
    plt.show()


def chart_event_and_angle_by_period_maintenance():
    info_by_turbine: dict = {}
    for (id_turbine, all_dates_by_turbine) in yield_get_all_date_event_by_turbine():
        if len(info_by_turbine) < 2:
            periods_run_to_failure = Util_Plt.create_date_run_to_failure(all_dates_by_turbine)
            all_angle: list = []
            for value in periods_run_to_failure:
                all_sensors = Util_Plt.create_dictionary_by_values([all_sensor for all_sensor in
                                                                    db_call.read_all_sensor_data_turbine_period(
                                                                        id_turbine, value.date_init,
                                                                        value.date_finish)])
                for key in all_sensors:
                    if key != 1:
                        all_sensors[key] = Util_Plt.filter_any_series_by_active_power(
                            {1: all_sensors[1], key: all_sensors[key]})
                all_sensors[1] = Util_Plt.remove_active_power_negative(all_sensors[1])
                info_by_turbine = get_info_sensor(info_by_turbine, id_turbine, all_sensors, value)
            unique_sensor_id = info_by_turbine[id_turbine]
            all_angle = Util_Plt.set_same_dimension(all_angle)
            info_by_turbine[id_turbine] = all_angle
    print_angle_all_turbine(info_by_turbine)


def get_info_sensor(info_by_turbine: dict, id_turbine: int, all_sensors: dict, value: str) -> dict:
    if id_turbine in info_by_turbine:
        for key in all_sensors:
            if (key, True) in info_by_turbine[id_turbine]:
                info_by_turbine[id_turbine][(key, True)].append(
                    [SensorInformation(key, val, "", True) for key in all_sensors for val
                     in all_sensors[key]])
            else:
                info_by_turbine[id_turbine][(key, True)] = [
                    [SensorInformation(key, val, "", True) for key in all_sensors
                     for val in all_sensors[key]]]
    else:
        info_by_turbine[id_turbine] = {}
        for key in all_sensors:
            info_by_turbine[id_turbine][(key, True)] = [[SensorInformation(key, val[0], "", True)
                                                         for val in all_sensors[key]]]
    for angle, id_sensor in Ch.get_angle(id_turbine, value):
        if id_turbine in info_by_turbine and (id_sensor, False) in info_by_turbine[id_turbine]:
            info_by_turbine[id_turbine][(id_sensor, False)].append(
                [SensorInformation(id_sensor, val, "", False) for val in angle])
        else:
            info_by_turbine[id_turbine][(id_sensor, False)] = [[SensorInformation(id_sensor, val, "", False)
                                                                for val in angle]]
    return info_by_turbine


def calculus_rul():
    pass
