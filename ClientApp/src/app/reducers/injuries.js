import Injuries from '../constants/injuries';

const initialState = {
  injuries: [],
  injuryLoader: true
};

export default (state = initialState, action = {}) => {
  switch (action.type) {
    case Injuries.GET_INJURIES:
      return {
        ...state,
        injuries: action.injuries,
        injuryLoader: action.injuryLoader
      };
    default:
      return state;
  }
};
