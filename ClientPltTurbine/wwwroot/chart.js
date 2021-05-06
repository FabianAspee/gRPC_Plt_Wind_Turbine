window.setupAreaChart = (id, config) => {
    var ctx = document.getElementById(id).getContext('2d');
    new Chart(ctx, config);
}
window.setupLineChart = (id, config) => {
    console.log(config)
    const footer = (tooltipItems) => { 
        let date = "Date :"
        tooltipItems.forEach(function (tooltipItem) {
            date = date.concat(tooltipItem.parsed.x+ "\t");
        });
        return date;
    };
    var ctx = document.getElementById(id).getContext('2d');
    config.options.plugins = {
        legend: {
            position: 'top',
        },
        tooltip: {
            callbacks: {
                footer: footer,
            }
        }
    }
    new Chart(ctx, config);
}

window.setupScatterChart = (id, config) => { 
    var ctx = document.getElementById(id).getContext('2d');
    config.options.scales = {
        xAxes: [{
            position: 'bottom',
            ticks: {
                userCallback: function (label, index, labels) {
                    console.log(label)
                    return moment(label).format("DD/MM/YY");
                }
            }
        }]
    }
    const footer = (tooltipItems) => {
        let date = "Date :"
        tooltipItems.forEach(function (tooltipItem) {
            date = date.concat(tooltipItem.parsed.x + "\t");
        });
        return date;
    };
    config.options.plugins = {
        legend: {
            position: 'top',
        },
        tooltip: {
            callbacks: {
                footer: footer,
            }
        }
    }
    new Chart(ctx, config);
}

window.setupRadarChart = (id, config) => {
    console.log(config)
    var ctx = document.getElementById(id).getContext('2d'); 
    new Chart(ctx, config);
} 

window.setupBoxPlotChart = (id, config) => {
    console.log(config)
    var ctx = document.getElementById(id).getContext('2d');
    new Chart(ctx, config);
} 

window.setupBarChart = (id, config) => {
    console.log(config)
    var ctx = document.getElementById(id).getContext('2d');
    new Chart(ctx, config);
} 