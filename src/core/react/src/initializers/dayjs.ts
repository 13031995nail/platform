import dayjs from 'dayjs';
import 'dayjs/locale/ru';
import isSameOrBefore from 'dayjs/plugin/isSameOrBefore';
import isSameOrAfter from 'dayjs/plugin/isSameOrAfter';
import customParseFormat from 'dayjs/plugin/customParseFormat';
import minMax from 'dayjs/plugin/minMax';
import utc from 'dayjs/plugin/utc';

dayjs.extend(isSameOrBefore);
dayjs.extend(isSameOrAfter);
dayjs.extend(customParseFormat);
dayjs.extend(minMax);
dayjs.extend(utc);

export default dayjs;
