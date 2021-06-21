from dataclasses import dataclass
from datetime import datetime


@dataclass(frozen=True)
class DateTurbine:
    date_init: str
    date_finish: str
    is_normal: bool


@dataclass(frozen=True)
class DateTurbineCustom:
    date_init: str
    date_finish: str


@dataclass(frozen=True)
class DateTurbineError:
    date_error: str
    error: float


@dataclass(frozen=True)
class DateTurbineErrorCustom:
    date_init: str
    date_finish: str
    error: float


@dataclass(frozen=True)
class TotalWarning:
    total_warning: int
    error_code: float


@dataclass(frozen=True)
class MaintenanceTurbine:
    id_turbine: int
    date_init: str
    date_finish: str
    is_normal: bool


@dataclass(frozen=True)
class ErrorInfo:
    error: float
    color: (int, int, int)
    dimension: float


@dataclass(frozen=True)
class WarningInfo:
    warning: float
    color: (int, int, int)
    dimension: float


@dataclass(frozen=True)
class InfoTurbine:
    lines: list
    date: list
    name: str


@dataclass(frozen=True)
class InfoTurbineAll:
    date: datetime
    qta_warning: int
    maintenance: list


@dataclass(frozen=True)
class NormalOrExtraMaintenance:
    name_turbine: str
    is_normal: bool


@dataclass(frozen=True)
class SensorInformation:
    id_sensor: int
    value: float
    date: str
    is_real: bool
