import calendar
import os
import json
import random
from datetime import datetime
from typing import List

from matplotlib import colors
from matplotlib.colors import LinearSegmentedColormap

from matplotlib import collections as matcoll
from numpy import mat
from sklearn.cluster import KMeans
from sklearn.datasets import make_blobs
from MyEncoder import MyEncoder
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


def read_json(path_file):
    with open(path_file) as json_file:
        return json.load(json_file)


def create_scatter3d(info_period_failure):
    # Fixing random state for reproducibility
    np.random.seed(19680801)
    # For each set of style and range settings, plot n random points in the box
    # defined by x in [23, 32], y in [0, 100], z in [zlow, zhigh].
    all_key = []
    for key in info_period_failure:
        all_keys = list(info_period_failure[key].keys())
        for value in range(len(all_keys)):
            if len(all_keys) - value >= 3:
                all_key.append([all_keys[value], all_keys[value + 1], all_keys[value + 2]])
            elif len(all_keys) - value == 2:
                all_key.append([all_keys[value], all_keys[value + 1], all_keys[0]])
            else:
                all_key.append([all_keys[value], all_keys[0], all_keys[1]])
    for keys in all_key:
        fig = plt.figure()
        ax = fig.add_subplot(projection='3d')
        ax = scatter3d(ax, info_period_failure, keys)
        plt.show()
    print()


def scatter3d(ax, dictionary, keys):
    xs = np.array([float(x) if x is not None else np.nan for x in
                   dictionary[next(iter(dictionary))][keys[0]]])
    ys = np.array([float(x) if x is not None else np.nan for x in
                   dictionary[next(iter(dictionary))][keys[1]]])
    zs = np.array([float(x) if x is not None else np.nan for x in
                   dictionary[next(iter(dictionary))][keys[2]]])
    ax.scatter(xs, ys, zs, marker='o')

    ax.set_xlabel('Nacelle Direction')
    ax.set_ylabel('Wind Direction')
    ax.set_zlabel('Wind Speed')
    plt.show()


def chart_create_scatter3d_nacelle_wind_active(info_period_failure):
    # Fixing random state for reproducibility
    np.random.seed(19680801)
    # For each set of style and range settings, plot n random points in the box
    # defined by x in [23, 32], y in [0, 100], z in [zlow, zhigh].
    all_keys = [['(1, True)', '(3, True)', '(5, True)'], ['(2, True)', '(4, True)', '(1, False)'],
                ['(1, True)', '(3, True)', '(1, False)'], ['(2, True)', '(4, True)', '(5, True)']]
    for keys in all_keys:
        fig = plt.figure()
        ax = fig.add_subplot(projection='3d')
        scatter3d(ax, info_period_failure, keys)


def create_cluster(info_period_failure):
    from sklearn import datasets

    np.random.seed(5)
    all_keys = [['(1, True)', '(3, True)', '(5, True)'], ['(2, True)', '(4, True)', '(1, False)'],
                ['(1, True)', '(3, True)', '(1, False)'], ['(2, True)', '(4, True)', '(5, True)']]
    for keys in all_keys:
        max_val = max(list(map(lambda val_f: len(info_period_failure[next(iter(info_period_failure))][val_f]),
                               map(lambda val: val, info_period_failure[next(iter(info_period_failure))]))))
        X = np.empty((max_val, 3))
        for index in range(len(keys)):
            values = info_period_failure[next(iter(info_period_failure))][keys[index]]
            if len(values) < max_val:
                values = values + [None for _ in range(max_val - len(values))]
            X[:, index] = [float(x) if x is not None else np.nan for x in values]
        col_mean = np.nanmean(X, axis=0)
        index_c = np.where(np.isnan(X))
        xx = X[:, 0]
        yy = X[:, 1]
        zz = X[:, 2]
        print(max(xx))
        print(max(yy))
        print(max(zz))
        for i,x in enumerate(zz,start=0):
            if x >10000:
                print()
        X[index_c] = np.take(col_mean, index_c[1])
        from sklearn.datasets import load_digits

        data, labels = load_digits(return_X_y=True)

        from sklearn.decomposition import PCA

        (n_samples, n_features), n_digits = data.shape, np.unique(labels).size
        reduced_data = PCA(n_components=2).fit_transform(data)
        # Place column means in the indices. Align the arrays using take
        from yellowbrick.cluster import KElbowVisualizer
        model = KMeans()
        # k is range of number of clusters.
        visualizer = KElbowVisualizer(model, k=(2, 30), timings=True)
        visualizer.fit(X)  # Fit data to visualizer
        visualizer.show()
        k_means = KMeans(n_clusters=visualizer.elbow_value_)
        k_means.fit_predict(X)
        fig = plt.figure()
        ax = fig.add_subplot(projection='3d')
        ax.plot(X[:, 0], X[:, 1], X[:, 2], 'k.', markersize=2)
        centroids = k_means.cluster_centers_
        ax.scatter(centroids[:, 0], centroids[:, 1], marker="x", s=169, linewidths=3,
                   color="b", zorder=10)
        ax.set_xlabel('Nacelle Direction')
        ax.set_ylabel('Wind Direction')
        ax.set_zlabel('Wind Speed')
        plt.show()


def group_info_turbine(dictionary, final_dictionary):
    for key_turbine in dictionary:
        for key_sensor in dictionary[key_turbine]:
            if not bool(final_dictionary):
                final_dictionary[key_turbine] = {}
            if key_sensor in final_dictionary[key_turbine]:
                final_dictionary[key_turbine][key_sensor] = final_dictionary[key_turbine][key_sensor] + \
                                                            dictionary[key_turbine][key_sensor]
            else:
                final_dictionary[key_turbine][key_sensor] = dictionary[key_turbine][key_sensor]
    return final_dictionary


def chart_event_and_angle_by_period_maintenance():
    for (id_turbine, all_dates_by_turbine) in yield_get_all_date_event_by_turbine():
        periods_run_to_failure = Util_Plt.create_date_run_to_failure(all_dates_by_turbine)
        final_dictionary = {}
        for value in periods_run_to_failure:
            file = f'{get_name_file(id_turbine, value)}.json'
            if os.path.exists(file):
                dictionary = read_json(file)
                final_dictionary = group_info_turbine(dictionary, final_dictionary)
        create_cluster(final_dictionary)
        chart_create_scatter3d_nacelle_wind_active(final_dictionary)


def save_in_json_event_and_angle_by_period_maintenance():
    for (id_turbine, all_dates_by_turbine) in yield_get_all_date_event_by_turbine():
        periods_run_to_failure = Util_Plt.create_date_run_to_failure(all_dates_by_turbine)
        for value in periods_run_to_failure:
            info_by_turbine: dict = {}
            all_sensors = Util_Plt.create_dictionary_by_values([all_sensor for all_sensor in
                                                                db_call.read_all_sensor_data_turbine_period(
                                                                    id_turbine, value.date_init,
                                                                    value.date_finish)])
            print(value)
            print(id_turbine, all_sensors.keys())
            if len(all_sensors.keys()) == 5:
                for key in all_sensors:
                    if key != 1:
                        all_sensors[key] = Util_Plt.filter_any_series_by_active_power(
                            {1: all_sensors[1], key: all_sensors[key]})

                all_sensors[1] = Util_Plt.remove_active_power_negative(all_sensors[1])
                info_by_turbine = get_info_sensor(info_by_turbine, id_turbine, all_sensors, value)
                print(len(all_sensors[next(iter(all_sensors.keys()))]))
                if len(all_sensors[next(iter(all_sensors.keys()))]) > 1:
                    calculus_rul(info_by_turbine, value)

    # print_angle_all_turbine(info_by_turbine)


def get_info_sensor(info_by_turbine: dict, id_turbine: int, all_sensors: dict, value: str) -> dict:
    if id_turbine in info_by_turbine:
        for key in all_sensors:
            if (key, True) in info_by_turbine[id_turbine]:
                info_by_turbine[id_turbine][(key, True)].append(
                    [SensorInformation(key, val, "", True) for val in all_sensors[key]])
            else:
                info_by_turbine[id_turbine][(key, True)] = [
                    [SensorInformation(key, val, "", True) for val in all_sensors[key]]]
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


__dataset__ = "dataset/"


def check_directory(key):
    if not os.path.exists(f"{__dataset__}{key}"):
        os.mkdir(f"{__dataset__}{key}")
        print("Directory ", f"{__dataset__}{key}", " Created ")
    else:
        print("Directory ", f"{__dataset__}{key}", " already exists")


def save_period_json(info_by_turbine, name_file):
    with open(f'{name_file}.json', 'w') as fp:
        new_dictionary = {}
        for k, v in info_by_turbine.items():
            new_dictionary[k] = {}
            for kk, vv in v.items():
                new_dictionary[k][str(kk)] = vv
        json.dump(new_dictionary, fp)


def get_name_file(key, period):
    date_init = datetime.strptime(period.date_init, Util_Plt.format_date)
    date_finish = datetime.strptime(period.date_finish, Util_Plt.format_date)
    return f'{__dataset__}{key}/{key}-{date_init.year}-{date_init.month}-{date_init.day}-{date_finish.year}-{date_finish.month}-{date_finish.day}'


def convert_internal_element_in_normal_values(all_turbine):
    for key in all_turbine:
        all_turbine[key] = list(map(lambda value: value.value, all_turbine[key][0]))
    return all_turbine


def calculus_rul(all_turbine: dict, period):
    key = next(iter(all_turbine))
    check_directory(key)
    internal_key = next(iter(all_turbine[key]))
    all_turbine[key] = convert_internal_element_in_normal_values(all_turbine[key])
    rul = np.array([x for x in reversed(range(len(all_turbine[key][internal_key])))])
    health_condition = rul / max(rul)
    all_turbine[key]["health_condition"] = list(health_condition)
    save_period_json(all_turbine, f'{get_name_file(key, period)}')


"""            for key in info_by_turbine:
                for key_sensor in info_by_turbine[key]:
                    info_by_turbine[key][key_sensor] = Util_Plt.set_same_dimension(info_by_turbine[key][key_sensor])"""
