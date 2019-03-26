import Injuries from '../constants/injuries';
import { getInjuries } from '../utils/networkFunctions';

export const loadInjuries = () => async (dispatch) => {
  await getInjuries().then((res) => {
    dispatch({
      type: Injuries.LOAD_INJURIES,
      injuries: res.data
    });
  });
};

export default loadInjuries;
