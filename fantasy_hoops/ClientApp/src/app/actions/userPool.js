import Users from '../constants/userPool';
import { getUsers } from '../utils/networkFunctions';

export const loadUserPool = () => async (dispatch) => {
  await getUsers().then((res) => {
    dispatch({
      type: Users.LOAD_USER_POOL,
      allUsers: res.data,
      filteredUsers: res.data
    });
  });
};

export const filterUsers = filterString => async (dispatch) => {
  dispatch({
    type: Users.FILTER_USERS,
    filterString
  });
};
