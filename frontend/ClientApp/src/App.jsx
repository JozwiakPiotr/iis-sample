import React, { useEffect, useState } from 'react'

export default function App(){
  const [users, setUsers] = useState([])
  const [name, setName] = useState('')

  useEffect(()=>{ fetch('/api/users').then(r=>r.json()).then(setUsers) }, [])

  const add = async ()=>{
    const r = await fetch('/api/users', {method:'POST', headers:{'Content-Type':'application/json'}, body: JSON.stringify({ name })})
    if(r.ok){ setName(''); setUsers(await (await fetch('/api/users')).json()) }
  }

  const del = async (id)=>{ await fetch('/api/users/'+id, {method:'DELETE'}); setUsers(users.filter(u=>u.id!==id)) }

  return (
    <div style={{padding:20}}>
      <h2>Users</h2>
      <input value={name} onChange={e=>setName(e.target.value)} placeholder="Name" />
      <button onClick={add}>Add</button>
      <ul>
        {users.map(u=> <li key={u.id}>{u.name} <button onClick={()=>del(u.id)}>Delete</button></li>)}
      </ul>
    </div>
  )
}
