from dataclasses import dataclass


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
