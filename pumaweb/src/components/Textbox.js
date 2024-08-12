import React, { useState, useEffect } from 'react';
export default function TextBox({
  onChange,
  value,
  className = '',
  id = '',
  name = '',
  readOnly = false,
  type = 'text'
}) {
  const _onChange = value => {
    try {
      onChange(value);
    } catch (e) {
      console.log('inputbox error', e);
    }
  };

  return (
    <input
      type={type}
      id={id}
      name={name}
      value={value}
      onChange={e => _onChange(e.target.value)}
    />
  );
}