import * as moment from "moment";

export const DatePickerTypes = {
    DAY: 'DAY',
    WEEK: 'WEEK'
}

export function makeJSDateObject(date) {
    if (moment.isMoment(date)) {
        return date.clone().toDate();
    }

    if (date instanceof Date) {
        return new Date(date.getTime());
    }

    throw new Error('Cannot properly parse argument passed to cloneCrossUtils');
}