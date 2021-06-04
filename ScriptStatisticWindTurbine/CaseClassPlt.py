
from dataclasses import dataclass

@dataclass(frozen=True)
class DateTurbine:
    date_init: str
    date_finish: str
    is_normal: bool

@dataclass(frozen=True)
class TotalWarning:
    total_warning: int
    error_code: float