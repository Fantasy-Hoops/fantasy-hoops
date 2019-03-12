import React from 'react';
import Img from 'react-image';
import defaultPhoto from '../../../content/images/default.png';

export const RequestCard = (props) => {
  const { id, userName, type, cancel, decline, accept } = props;

  const img = new Image();
  let avatar;
  if (id) {
    img.src = `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${id}.png`;
    avatar = img.height !== 0 ? img.src : defaultPhoto;
  }
  return (
    <div className="card rounded mx-auto RequestCard">
      <div className="RequestCard__Body">
        <a href={`/profile/${userName}`} className="card-body RequestCard__Info">
          <Img
            className="RequestCard__Avatar"
            alt=""
            src={avatar}
            decode={false}
          />
          <p className="RequestCard__Info__Name">{userName}</p>
        </a>
        <div className="RequestCard__Buttons">
          {
            type === 1
              ? (
                <button type="button" onClick={() => cancel(id)} className="btn btn-outline-danger">Cancel Request</button>
              )
              : (
                <div>
                  <button type="button" onClick={() => decline(id)} className="btn btn-outline-danger">Decline Request</button>
                  <button type="button" onClick={() => accept(id)} className="btn btn-success ml-3">Accept Request</button>
                </div>
              )
          }
        </div>
      </div>
    </div>
  );
};

export default RequestCard;
