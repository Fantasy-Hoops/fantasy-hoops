import React from 'react';
import { Link } from 'react-router-dom';
import PropTypes from 'prop-types';
import { parse } from '../../../utils/auth';
import FriendList from './FriendList';
import RequestList from './RequestList';

export const Friends = (props) => {
  const { user } = props;
  const loggedInAsSameUser = (user.userName != null
    && parse().username.toLowerCase() === user.userName.toLowerCase());
  return (
    <div className="tab-pane" id="friends">
      <div className="container p-2">
        {loggedInAsSameUser && (
          <ul className="nav nav-pills mb-3" id="pills-tab" role="tablist">
            <li className="nav-item">
              <Link className="nav-link active" id="pills-friends-tab" data-toggle="pill" to="#pills-friends" role="tab" aria-controls="pills-friends" aria-selected="true">My friends</Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link" id="pills-requests-tab" data-toggle="pill" to="#pills-requests" role="tab" aria-controls="pills-requests" aria-selected="false">Requests</Link>
            </li>
          </ul>
        )}
        <div className="tab-content" id="pills-tabContent">
          <div className="tab-pane fade show active" id="pills-friends" role="tabpanel" aria-labelledby="pills-friends-tab">
            <FriendList user={user} />
          </div>
          <div className="tab-pane fade" id="pills-requests" role="tabpanel" aria-labelledby="pills-requests-tab">
            <RequestList user={user} />
          </div>
        </div>
      </div>
    </div>
  );
};

Friends.propTypes = {
  user: PropTypes.shape({
    userName: PropTypes.string.isRequired
  }).isRequired
};

export default Friends;
