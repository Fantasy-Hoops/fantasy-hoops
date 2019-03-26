import _ from 'lodash';
import Users from '../constants/userPool';

const initialState = {
  allUsers: [],
  filteredUsers: [],
  filterString: '',
  userPoolLoader: true,
};

export default (state = initialState, action = {}) => {
  switch (action.type) {
    case Users.LOAD_USER_POOL:
      return {
        ...state,
        allUsers: action.allUsers,
        filteredUsers: action.filteredUsers,
        userPoolLoader: false
      };
    case Users.FILTER_USERS:
      return {
        ...state,
        filteredUsers: _.filter(state.allUsers,
          user => user.userName.toLowerCase()
            .search(action.filterString) !== -1)
      };
    default:
      return state;
  }
};
