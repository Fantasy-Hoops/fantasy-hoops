import React, { Component } from 'react';
import { UserCard } from './../UserCard';
import shortid from 'shortid';
import defaultPhoto from '../../../content/images/avatars/default.png';
import _ from 'lodash';

export class FriendList extends Component {
  constructor(props) {
    super(props);
    this.state = {
      friends: ''
    }
  }

  componentDidUpdate(prevProps) {
    if (prevProps === this.props)
      return;

    fetch(`http://68.183.213.191:5001/api/user/friends/${this.props.user.id}`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          friends: res
        })
      });

  }

  render() {
    let friends = _.map(this.state.friends,
      (friend) => {
        let avatar;
        try {
          require(`../../../content/images/avatars/${friend.id}`);
        } catch (err) {
          avatar = defaultPhoto;
        }
        return <UserCard
          key={shortid()}
          userName={friend.userName}
          avatar={avatar}
          color={friend.color}
        />
      }
    );
    return (
      <div className="row">
        <div className="row">
          {friends.length > 0 ? friends : "User doesn't have any friends!"}
        </div>
      </div>
    );
  }
}